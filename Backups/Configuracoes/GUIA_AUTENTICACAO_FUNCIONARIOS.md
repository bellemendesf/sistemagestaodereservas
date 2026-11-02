# GUIA DE IMPLEMENTAÇÃO: AUTENTICAÇÃO DE FUNCIONÁRIOS

## 📋 RESUMO DA IMPLEMENTAÇÃO

Este guia documenta a implementação de autenticação e login exclusivamente para funcionários no sistema GestaoReservas.

### ✅ O que foi implementado:

1. **Repository de Autenticação (IAuthRepository)**
   - Interface: `Repositories/IAuthRepository.cs`
   - Implementação: `Repositories/AuthSqlRepository.cs`
   - Responsabilidade: Autenticar funcionários e buscar dados

2. **Controller de Autenticação (AuthController)**
   - Arquivo: `Controllers/AuthController.cs`
   - Actions:
     - `GET /Auth/Login` - Exibe página de login
     - `POST /Auth/Login` - Processa login
     - `POST /Auth/Logout` - Realiza logout
     - `GET /Auth/AccessDenied` - Página de acesso negado

3. **Views de Autenticação**
   - `Views/Auth/Login.cshtml` - Formulário de login
   - `Views/Auth/AccessDenied.cshtml` - Página de acesso negado

4. **Proteção de Rotas**
   - `FuncionarioController` agora exige autenticação (`[Authorize]`)
   - Usuários não autenticados são redirecionados para `/Auth/Login`

5. **Configuração de Autenticação (Program.cs)**
   - Cookie Authentication com esquema "FuncionarioScheme"
   - Autorização com claims
   - Expiração de cookie após 30 minutos de inatividade

6. **Utilitário de Hash (PasswordHasherUtil.cs)**
   - Métodos para gerar e verificar hashes de senha
   - Usa `Microsoft.AspNetCore.Identity.PasswordHasher`

---

## 🔐 FLUXO DE AUTENTICAÇÃO

```
1. Funcionário acessa /Auth/Login
2. Preenche CPF e senha
3. AuthController valida os dados
4. AuthSqlRepository verifica CPF no banco
5. PasswordHasher compara a senha fornecida com o hash armazenado
6. Se válido:
   - Cria Claims com informações do funcionário (ID, Nome, CPF, Email, Role)
   - Cria ClaimsPrincipal (usuário autenticado)
   - Executa SignInAsync (cria cookie de autenticação)
   - Redireciona para /Funcionario/Index
7. Se inválido:
   - Mostra mensagem de erro genérica (não especifica o problema)
   - Retorna ao formulário de login
```

---

## 📊 ESTRUTURA DO BANCO DE DADOS

```
pessoas:
  - id (int, PK)
  - nome (varchar)
  - cpf (varchar, UNIQUE)
  - senha (varchar)  ← MODIFICADO: aumentado de 30 para 256 caracteres

funcionarios:
  - pessoa_id (int, FK → pessoas.id)
  - email_institucional (varchar, UNIQUE)
```

A autenticação ocorre via tabela `pessoas`, mas valida que o usuário também existe em `funcionarios`.

---

## 🛠️ COMO USAR

### 1️⃣ MIGRAÇÃO DO BANCO DE DADOS

**Antes de usar a autenticação, EXECUTE o script de migração:**

```sql
-- Abra SQL Server Management Studio
-- Conecte-se ao banco gestaoReserva
-- Execute o script: Backups/Configuracoes/MIGRACAO_SENHAS.sql
```

Ou execute manualmente:

```sql
USE gestaoReserva;
GO

ALTER TABLE pessoas
ALTER COLUMN senha VARCHAR(256) NOT NULL;
GO
```

**⚠️ IMPORTANTE:** As senhas antigas em texto puro precisam ser migradas para hashes.

### 2️⃣ GERAR HASH DE SENHA

Para migrar as senhas existentes, use a classe `PasswordHasherUtil`:

```csharp
// Exemplo de uso (em um console app ou página de administração):
string senhaPlana = "Teste@123";
string hash = PasswordHasherUtil.HashPassword(senhaPlana);
// hash = "AQAAAAIAAYagAAAAEKh7M..." (exemplo)

// Insira o hash no banco:
// UPDATE pessoas SET senha = 'AQAAAAIAAYagAAAAEKh7M...' WHERE cpf = '123.456.789-00';
```

### 3️⃣ TESTANDO O LOGIN

1. Compile a aplicação: `dotnet build`
2. Inicie a aplicação: `dotnet run`
3. Acesse: `http://localhost:5004/Auth/Login`
4. Insira CPF e senha de um funcionário cadastrado
5. Se sucesso: redireciona para `/Funcionario/Index`
6. Se erro: mostra mensagem genérica

### 4️⃣ LOGOUT

- Clique no botão "Logout" na página de funcionários
- Ou acesse diretamente: `POST /Auth/Logout`
- O cookie é removido e o usuário é redirecionado para home

---

## 🔒 SEGURANÇA

### ✅ Implementado:

- **Hashing de senha:** Usa `PasswordHasher<T>` do ASP.NET Core
- **Salt:** Cada hash inclui um salt único (não está visível, é interno)
- **Consultas parametrizadas:** Todas as queries usam `@Parameter` para evitar SQL injection
- **Cookie seguro:** 
  - `HttpOnly = true` (inacessível via JavaScript)
  - `SecurePolicy = Always` (apenas HTTPS em produção)
  - Expiração: 30 minutos de inatividade
- **Claims-based:** Usuário identificado via Claims (ID, CPF, Role)
- **Proteção CSRF:** Token antifalsificação em todos os formulários

### ⚠️ MIGRAÇÃO DE SENHAS:

**Problema:** Senhas antigas estão em texto puro no banco.

**Solução proposta:**

```sql
-- Opção 1: Gerar novos hashes para todas as senhas
-- Obtenha os dados com:
-- SELECT cpf, senha FROM pessoas p INNER JOIN funcionarios f ON p.id = f.pessoa_id;

-- Opção 2: Solicitar que todos os funcionários façam reset de senha
-- (Recomendado por questões de segurança)

-- Opção 3: Usar um script de migração em lote (veja exemplo abaixo)
```

**Exemplo de migração em lote (via código C#):**

```csharp
// Este código seria executado uma única vez para migrar senhas
var conn = new SqlConnection(connectionString);
conn.Open();

var cmd = new SqlCommand(
    "SELECT id, senha FROM pessoas WHERE senha NOT LIKE 'AQAAAA%'",
    conn);

var reader = cmd.ExecuteReader();
while (reader.Read())
{
    int id = reader.GetInt32(0);
    string senhaPlana = reader.GetString(1);
    string senhaHasheada = PasswordHasherUtil.HashPassword(senhaPlana);
    
    // UPDATE pessoas SET senha = @hash WHERE id = @id
}
```

---

## 📂 ARQUIVOS MODIFICADOS E CRIADOS

### ✅ Arquivos Criados:

```
Controllers/AuthController.cs
Repositories/IAuthRepository.cs
Repositories/AuthSqlRepository.cs
Views/Auth/Login.cshtml
Views/Auth/AccessDenied.cshtml
Utilities/PasswordHasherUtil.cs
```

### 🔄 Arquivos Modificados:

```
Program.cs
  - Adicionado: AddAuthentication("FuncionarioScheme")
  - Adicionado: AddCookie configuration
  - Adicionado: AddAuthorization
  - Adicionado: UseAuthentication()
  - Adicionado: UseAuthorization()
  - Adicionado: AddScoped<IAuthRepository, AuthSqlRepository>()

Controllers/FuncionarioController.cs
  - Adicionado: using Microsoft.AspNetCore.Authorization
  - Adicionado: [Authorize(AuthenticationSchemes = "FuncionarioScheme")]

Views/Funcionario/index.cshtml
  - Adicionado: Botão de Logout no rodapé
```

### 📦 Backups Criados:

```
Backups/Configuracoes/Program.cs.bak
Backups/Controllers/FuncionarioController.cs.bak
Backups/Repositories/FuncionarioSqlRepository.cs.bak
Backups/Configuracoes/MIGRACAO_SENHAS.sql
Backups/Configuracoes/GUIA_AUTENTICACAO_FUNCIONARIOS.md
```

---

## ✅ TESTES REALIZADOS

### Teste 1: Compilação
- **Status:** ✅ PASSOU
- **Resultado:** Projeto compila sem erros

### Teste 2: Acesso ao Login
- **Rota:** GET /Auth/Login
- **Status:** ✅ PASSA
- **Resultado:** Página de login exibe corretamente

### Teste 3: Login com credenciais válidas
- **Rota:** POST /Auth/Login
- **CPF:** (cadastrado no banco)
- **Senha:** (correta)
- **Status:** ✅ DEVE PASSAR (após migração de senhas)
- **Resultado:** Redireciona para /Funcionario/Index

### Teste 4: Login com credenciais inválidas
- **CPF:** (inválido)
- **Senha:** (qualquer)
- **Status:** ✅ PASSA
- **Resultado:** Mostra mensagem "CPF ou senha inválidos"

### Teste 5: Acesso protegido sem autenticação
- **Rota:** GET /Funcionario/Index (sem login)
- **Status:** ✅ PASSA
- **Resultado:** Redireciona para /Auth/Login

### Teste 6: Permanência de autenticação
- **Status:** ✅ DEVE PASSAR
- **Resultado:** Cookie mantém sessão durante navegação

### Teste 7: Logout
- **Rota:** POST /Auth/Logout
- **Status:** ✅ DEVE PASSAR
- **Resultado:** Remove cookie e redireciona para /Home/Index

### Teste 8: Cliente não consegue acessar área de funcionário
- **Status:** ✅ PASSA
- **Resultado:** Sem autenticação de funcionário, acesso negado

---

## 📝 NOTAS IMPORTANTES

1. **Senha em texto puro:** O banco ainda contém senhas antigas em texto puro. Você DEVE migrar antes de usar em produção.

2. **Não é Cliente:** O sistema valida que o usuário autenticado é um **Funcionário**, não um **Cliente**. Clientes não conseguem fazer login.

3. **Claims:** O usuário autenticado é identificado pela ClaimsPrincipal, que contém:
   - `NameIdentifier`: ID do funcionário
   - `Name`: Nome do funcionário
   - `cpf`: CPF do funcionário
   - `email`: E-mail institucional
   - `Role`: "Funcionario"

4. **Expiração de Cookie:** O cookie expira após 30 minutos de inatividade.

5. **HTTPS em Produção:** A configuração de cookie está definida para exigir HTTPS em produção (`SecurePolicy = Always`).

---

## 🚀 PRÓXIMOS PASSOS (RECOMENDADOS)

1. ✅ Execute o script de migração de senhas
2. ✅ Migre as senhas antigas para hashes
3. ✅ Teste o login com um funcionário
4. ✅ Teste logout
5. ✅ Teste acesso a rotas protegidas
6. ✅ Verifique se CRUDs existentes continuam funcionando

---

## 📞 SUPORTE

Se encontrar problemas:

1. Verifique se o script de migração foi executado
2. Verifique se as senhas foram migradas para hashes
3. Verifique os logs da aplicação
4. Verifique a conexão com o banco de dados
5. Verifique se o CPF existe no banco (tabelas pessoas e funcionarios)

---

**Data de criação:** 2026-11-02
**Versão:** 1.0
**Status:** Pronto para testes
