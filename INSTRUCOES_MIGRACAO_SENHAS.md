# 🔐 INSTRUÇÕES DE MIGRAÇÃO DE SENHAS

## ⚠️ CRÍTICO: EXECUTE ANTES DE USAR A AUTENTICAÇÃO

Este documento contém as instruções **obrigatórias** para migrar as senhas existentes de texto puro para hashes seguros.

---

## 📋 PRÉ-REQUISITOS

- SQL Server Management Studio (ou acesso ao SQL Server)
- Acesso ao banco `gestaoReserva`
- Permissão para executar ALTER TABLE
- Cópia de segurança do banco de dados (recomendado)

---

## 🚨 ENTENDA O PROBLEMA

### Situação Atual
```sql
-- Senhas armazenadas em texto puro
SELECT cpf, senha FROM pessoas;

cpf              | senha
-----------------|---------
123.456.789-00   | 123456
111.222.333-44   | Teste@123
```

### Por que isso é inseguro?
- Qualquer pessoa com acesso ao banco vê as senhas
- Se o banco for comprometido, todas as senhas são expostas
- Não segue padrões de segurança

### Situação Após Migração
```sql
-- Senhas armazenadas como hash (impossível reverter)
SELECT cpf, senha FROM pessoas;

cpf              | senha
-----------------|---------------------------------------------------------
123.456.789-00   | AQAAAAIAAYagAAAAEKh7M/wvH5+IiBXF1A...
111.222.333-44   | AQAAAAIAAYagAAAAELk9N/xyJ7+JmCIW2F...
```

- Apenas hash criptografado (não pode ser revertido)
- Mesmo que o banco seja comprometido, senhas não são expostas
- PasswordHasher verifica a senha sem revelar o hash

---

## ✅ PASSO 1: AUMENTAR A COLUNA SENHA

A coluna `senha` atual é `varchar(30)`, mas hashes precisam de `varchar(256)`.

### Execute no SQL Server:

```sql
-- Conecte-se ao banco gestaoReserva
USE gestaoReserva;
GO

-- Aumentar coluna senha de 30 para 256 caracteres
ALTER TABLE pessoas
ALTER COLUMN senha VARCHAR(256) NOT NULL;
GO

-- Confirmar que funcionou
EXEC sp_columns 'pessoas';
GO
```

**Resultado esperado:**
```
coluna_name: senha
tipo_dados: varchar
tamanho: 256
```

---

## ✅ PASSO 2: LISTAR FUNCIONÁRIOS COM SENHAS ANTIGAS

Primeiro, identifique quais funcionários precisam ter a senha migrada.

```sql
-- Listar todos os funcionários
SELECT 
    p.id,
    p.nome,
    p.cpf,
    p.senha,
    f.email_institucional
FROM pessoas p
INNER JOIN funcionarios f ON p.id = f.pessoa_id
ORDER BY p.id;
```

**Exemplo de saída:**
```
id  | nome              | cpf             | senha      | email_institucional
----|-------------------|-----------------|----------|---------------------
1   | João Silva        | 123.456.789-00  | 123456   | joao@santamassa.br
2   | Maria Santos      | 111.222.333-44  | Teste@123| maria@santamassa.br
3   | Pedro Oliveira    | 555.666.777-88  | abc123   | pedro@santamassa.br
```

**Guarde essas informações**, você precisará delas para gerar os hashes.

---

## ✅ PASSO 3: GERAR HASHES DAS SENHAS

### Opção A: Usar C# para gerar hashes

```csharp
using Microsoft.AspNetCore.Identity;
using GestaoReservas.Models;

// Instanciar o PasswordHasher
var hasher = new PasswordHasher<Funcionario>();

// Para cada funcionário, gerar o hash da senha
string senhaAntiga = "123456"; // Senha original em texto puro
var funcionarioTemp = new Funcionario();
string senhaHasheada = hasher.HashPassword(funcionarioTemp, senhaAntiga);

Console.WriteLine($"Senha original: {senhaAntiga}");
Console.WriteLine($"Senha hashada: {senhaHasheada}");

// Exemplo de saída:
// Senha original: 123456
// Senha hashada: AQAAAAIAAYagAAAAEKh7M/wvH5+IiBXF1A2G/hCXYk+Z0pKQI3DhKgZoR5OVwWGBV...
```

### Opção B: Usar o PasswordHasherUtil incluído

```csharp
// Se você compilou o projeto, pode usar o utilitário incluído
using GestaoReservas.Utilities;

string senhaAntiga = "123456";
string senhaHasheada = PasswordHasherUtil.HashPassword(senhaAntiga);

Console.WriteLine($"Hash gerado: {senhaHasheada}");
```

### Opção C: Usar uma página ASP.NET temporária

Crie uma página de administração temporária que gera hashes (recomendado para lote):

```csharp
[HttpPost]
public IActionResult GerarHash(string senha)
{
    var hasher = new PasswordHasher<Funcionario>();
    var temp = new Funcionario();
    string hash = hasher.HashPassword(temp, senha);
    
    return Json(new { senha = senha, hash = hash });
}
```

---

## ✅ PASSO 4: ATUALIZAR SENHAS NO BANCO

### Opção 1: Uma por uma (seguro)

```sql
-- Substitua os valores pelos dados reais
UPDATE pessoas
SET senha = 'AQAAAAIAAYagAAAAEKh7M/wvH5+IiBXF1A...'  -- Hash gerado
WHERE cpf = '123.456.789-00';  -- CPF do funcionário

GO

-- Verificar se atualizou
SELECT cpf, senha FROM pessoas WHERE cpf = '123.456.789-00';
```

**Repita para cada funcionário:**

```sql
-- Funcionário 2
UPDATE pessoas
SET senha = 'AQAAAAIAAYagAAAAELk9N/xyJ7+JmCIW2F...'
WHERE cpf = '111.222.333-44';
GO

-- Funcionário 3
UPDATE pessoas
SET senha = 'AQAAAAIAAYagAAAAEHo8O/wvI6+KkDYX3G...'
WHERE cpf = '555.666.777-88';
GO
```

### Opção 2: Script de atualização em lote

Se tiver muitos funcionários, use um script:

```sql
-- BACKUP ANTES DE EXECUTAR!
-- Este script usa um cursor para atualizar todas as senhas em lote

DECLARE @id INT;
DECLARE @senhaAntiga VARCHAR(30);
DECLARE @senhaNovaHash VARCHAR(256);

-- Criar tabela temporária com os hashes gerados
CREATE TABLE #SenhasHasheadas (
    id INT,
    senhaAntiga VARCHAR(30),
    senhaHasheada VARCHAR(256)
);

-- Preencher tabela com hashes (você obtém esses valores via C#)
INSERT INTO #SenhasHasheadas VALUES
(1, '123456', 'AQAAAAIAAYagAAAAEKh7M/wvH5+IiBXF1A...'),
(2, 'Teste@123', 'AQAAAAIAAYagAAAAELk9N/xyJ7+JmCIW2F...'),
(3, 'abc123', 'AQAAAAIAAYagAAAAEHo8O/wvI6+KkDYX3G...');

-- Atualizar tabela pessoas
UPDATE pessoas
SET senha = sh.senhaHasheada
FROM #SenhasHasheadas sh
WHERE pessoas.id = sh.id;

-- Limpar tabela temporária
DROP TABLE #SenhasHasheadas;

-- Verificar resultado
SELECT cpf, senha FROM pessoas;
```

### Opção 3: Verificar se está tudo correto

```sql
-- Verificar se todas as senhas foram migradas
SELECT 
    COUNT(*) as total_funcionarios,
    COUNT(CASE WHEN LEN(senha) > 100 THEN 1 END) as com_hash,
    COUNT(CASE WHEN LEN(senha) < 50 THEN 1 END) as sem_hash
FROM pessoas p
INNER JOIN funcionarios f ON p.id = f.pessoa_id;
```

**Resultado esperado:**
```
total_funcionarios | com_hash | sem_hash
------------------|----------|----------
3                  | 3        | 0
```

Se `sem_hash > 0`, ainda existem senhas antigas.

---

## ✅ PASSO 5: TESTAR O LOGIN

### Teste 1: Compilar a aplicação

```bash
cd C:\Users\isabe\GestaoReservas
dotnet build
```

**Resultado esperado:**
```
Compilação com êxito.
```

### Teste 2: Executar a aplicação

```bash
dotnet run
```

**A aplicação deve iniciar normalmente.**

### Teste 3: Testar login

1. Abra o navegador
2. Acesse: `http://localhost:5004/Auth/Login`
3. Preencha:
   - CPF: (de um funcionário cadastrado)
   - Senha: (a senha ORIGINAL, não o hash)
4. Clique em "Entrar"

**Resultado esperado:**
- ✅ Se correto: Redireciona para `/Funcionario/Index`
- ❌ Se errado: Mostra erro "CPF ou senha inválidos"

### Teste 4: Testar logout

1. Na página de funcionários, clique em "Logout"
2. Deve redirecionar para a página inicial
3. Tente acessar `/Funcionario/Index` novamente
4. Deve redirecionar para login (não autenticado)

---

## 🚨 PROBLEMAS COMUNS

### Problema 1: "CPF ou senha inválidos" mesmo com dados corretos

**Causa:** Senha não foi migrada para hash

**Solução:**
```sql
-- Verificar se a senha foi atualizada
SELECT cpf, LEFT(senha, 50) as primeiro_50_caracteres
FROM pessoas
WHERE cpf = '123.456.789-00';

-- Se não começar com "AQAAAA...", a migração não funcionou
```

### Problema 2: "Erro: O comprimento do valor excede o tamanho máximo de 30 caracteres"

**Causa:** Coluna `senha` ainda é `varchar(30)`

**Solução:**
```sql
-- Aumentar a coluna novamente
ALTER TABLE pessoas
ALTER COLUMN senha VARCHAR(256) NOT NULL;
```

### Problema 3: Nenhum funcionário consegue fazer login

**Causa:** Nenhuma senha foi migrada

**Solução:**
```sql
-- Verificar senhas no banco
SELECT cpf, LEN(senha) as tamanho_senha
FROM pessoas p
INNER JOIN funcionarios f ON p.id = f.pessoa_id;

-- Se tamanho_senha < 50, senhas não foram migradas
```

### Problema 4: Algum funcionário não consegue fazer login

**Causa:** Essa senha específica não foi migrada corretamente

**Solução:**
1. Gere o hash novamente
2. Atualize apenas aquela senha
3. Teste novamente

---

## ✅ CHECKLIST DE MIGRAÇÃO

- [ ] Fiz backup do banco de dados
- [ ] Aumentei coluna `senha` para `varchar(256)`
- [ ] Listei todos os funcionários com senhas
- [ ] Gerei hashes para todas as senhas
- [ ] Atualizei todas as senhas no banco
- [ ] Verifiquei que todas as senhas foram migradas
- [ ] Compilei a aplicação
- [ ] Testei login com um funcionário
- [ ] Testei logout
- [ ] Tentei acessar /Funcionario/Index sem autenticação
- [ ] Verifiquei que CRUDs existentes funcionam

---

## 📝 EXEMPLO PRÁTICO COMPLETO

### Cenário: 3 funcionários precisam de migração

#### Passo 1: Identificar funcionários
```sql
SELECT id, cpf, senha FROM pessoas p
INNER JOIN funcionarios f ON p.id = f.pessoa_id;

-- Resultado:
-- id=1, cpf=123.456.789-00, senha=123456
-- id=2, cpf=111.222.333-44, senha=Teste@123
-- id=3, cpf=555.666.777-88, senha=abc123
```

#### Passo 2: Gerar hashes (em C#)
```csharp
// Gere um hash para cada senha
PasswordHasherUtil.HashPassword("123456");     // → HASH1
PasswordHasherUtil.HashPassword("Teste@123");  // → HASH2
PasswordHasherUtil.HashPassword("abc123");     // → HASH3
```

#### Passo 3: Atualizar banco
```sql
UPDATE pessoas SET senha = 'HASH1' WHERE id = 1;
UPDATE pessoas SET senha = 'HASH2' WHERE id = 2;
UPDATE pessoas SET senha = 'HASH3' WHERE id = 3;
```

#### Passo 4: Testar
```
Login com CPF 123.456.789-00 e senha 123456 → ✅ Acesso permitido
Login com CPF 123.456.789-00 e senha errada → ❌ Acesso negado
```

---

## 🔒 BOAS PRÁTICAS

1. **Sempre fazer backup antes de alterar o banco**
2. **Testar com um funcionário antes de fazer todos**
3. **Guardar log de quais senhas foram migradas**
4. **Notificar funcionários sobre a mudança**
5. **Nunca compartilhar hashes com ninguém**
6. **Usar senhas fortes (mínimo 8 caracteres)**

---

## 📞 SUPORTE

Se algo der errado:

1. Restaure o backup do banco
2. Verifique o arquivo de logs
3. Releia as instruções com cuidado
4. Teste com um funcionário apenas

---

## ✅ CONCLUSÃO

Após seguir todos esses passos:
- ✅ Senhas são armazenadas de forma segura
- ✅ Login funcionará corretamente
- ✅ Logout funcionará
- ✅ Rotas protegidas funcionarão

**Próximo passo:** Testar o sistema completo em ambiente.

---

**Data:** 2026-11-02  
**Versão:** 1.0  
**Status:** Pronto para execução
