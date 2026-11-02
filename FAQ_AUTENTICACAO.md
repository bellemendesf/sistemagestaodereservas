# ❓ FAQ - PERGUNTAS FREQUENTES SOBRE AUTENTICAÇÃO

## Categoria: Login e Autenticação

### P1: Por que preciso fazer login?
**R:** Para proteger a área de gerenciamento de funcionários. Apenas usuários autenticados conseguem acessar, editar e deletar funcionários. Clientes não têm acesso a essa área.

### P2: O que é um hash de senha?
**R:** É uma transformação irreversível de uma senha. Se a senha é "Teste@123", seu hash é "AQAAAA...". Mesmo que alguém veja o hash, não consegue descobrir a senha. O sistema compara a senha fornecida com o hash para validar.

### P3: Por que não armazenar a senha em texto puro?
**R:** Se o banco for comprometido, todas as senhas seriam expostas. Com hash, mesmo que o banco vaze, as senhas são inúteis sem salt/iterações.

### P4: O sistema pode descobrir minha senha pelo hash?
**R:** Não. O hash é irreversível. Mesmo o administrador do sistema não consegue ver sua senha. Se esqueceu, precisa resetar.

### P5: Qual é o email para login?
**R:** Não é o email, é o **CPF**. O CPF é único para cada funcionário.

### P6: Como faço para resetar minha senha se esqueci?
**R:** Contacte o administrador. Neste escopo, não foi implementado sistema de reset automatizado.

---

## Categoria: Segurança

### P7: As senhas antigas em texto puro serão migradas automaticamente?
**R:** Não. Você deve executar manualmente o script de migração. Leia `INSTRUCOES_MIGRACAO_SENHAS.md`.

### P8: Por que migrar as senhas?
**R:** As senhas antigas estão em texto puro, o que é inseguro. A migração transforma todas para hash seguro.

### P9: Posso alterar a senha após o hash ser criado?
**R:** Sim, mas não há interface implementada para isso neste escopo. Seria necessário editar via administrador.

### P10: O cookie é seguro?
**R:** Sim. Está configurado com:
- `HttpOnly`: Inacessível via JavaScript
- `Secure`: Apenas HTTPS (em produção)
- `SameSite=Lax`: Proteção contra CSRF

### P11: Por quanto tempo a sessão durará?
**R:** 30 minutos de inatividade. Após esse tempo, o cookie expira e você precisa fazer login novamente.

### P12: Posso aumentar o tempo de expiração?
**R:** Sim, modificando `ExpiresUtc` em `AuthController.cs` ou em `Program.cs`.

---

## Categoria: Técnica

### P13: Qual é o esquema de autenticação?
**R:** "FuncionarioScheme". É um Cookie Authentication com esse nome específico.

### P14: Como o sistema sabe que sou um funcionário e não um cliente?
**R:** O repository busca no banco se o CPF existe em **duas** tabelas:
1. `pessoas` (dados comuns)
2. `funcionarios` (dados específicos)

Se o CPF existe em clientes, não consegue fazer login como funcionário.

### P15: O que é uma Claim?
**R:** É uma informação sobre o usuário autenticado. Exemplos:
- `NameIdentifier`: ID do funcionário
- `Name`: Nome completo
- `cpf`: CPF
- `email`: E-mail institucional
- `Role`: Tipo de usuário ("Funcionario")

### P16: Como acessar as informações do usuário em uma Action?
**R:** 
```csharp
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
var name = User.FindFirst(ClaimTypes.Name)?.Value;
var cpf = User.FindFirst("cpf")?.Value;
```

### P17: Como proteger outras rotas com autenticação?
**R:** Adicione o atributo em qualquer controller:
```csharp
[Authorize(AuthenticationSchemes = "FuncionarioScheme")]
public class OutroController : Controller { }
```

### P18: Posso desabilitar autenticação em alguma action?
**R:** Sim, use `[AllowAnonymous]`:
```csharp
[AllowAnonymous]
public ActionResult Login() { }
```

---

## Categoria: Problemas

### P19: Estou recebendo "CPF ou senha inválidos" mesmo com dados corretos
**Solução:**
1. Verifique se a senha foi migrada para hash
2. Confirme que o CPF existe no banco
3. Verifique a conexão com o banco

```sql
SELECT * FROM pessoas WHERE cpf = '123.456.789-00';
```

### P20: Recebo erro 404 em /Auth/Login
**Solução:**
1. Recompile: `dotnet build`
2. Verifique se `AuthController.cs` existe
3. Verifique se não há erro de sintaxe

### P21: O cookie não está sendo criado
**Solução:**
1. Verifique se o navegador aceita cookies
2. Verifique se a autenticação bem-sucedida acontece
3. Veja logs de erro da aplicação

### P22: Não consigo fazer logout
**Solução:**
1. Verifique se o botão "Logout" está visível
2. Verifique se o form está enviando corretamente
3. Verifique se a action `Logout` existe

### P23: Acesso protegido não está bloqueando usuários anônimos
**Solução:**
1. Verifique se `[Authorize]` está na classe ou método
2. Verifique se `UseAuthentication()` está em `Program.cs`
3. Verifique se `UseAuthorization()` está em `Program.cs`

---

## Categoria: Configuração

### P24: Como mudar o tempo de expiração do cookie?
**R:** Em `Program.cs`:
```csharp
options.ExpireTimeSpan = TimeSpan.FromHours(1); // 1 hora
```

### P25: Como mudar a rota de login padrão?
**R:** Em `Program.cs`:
```csharp
options.LoginPath = "/Auth/MinhaRotaDeLogin";
```

### P26: Como mudar o nome do cookie?
**R:** Em `Program.cs`:
```csharp
options.Cookie.Name = "MeuNovoNomeDeCookie";
```

### P27: Como exigir HTTPS em desenvolvimento?
**R:** Em `Program.cs`:
```csharp
options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
```

### P28: Como permitir acesso a uma action sem autenticação?
**R:** Use `[AllowAnonymous]`:
```csharp
[AllowAnonymous]
public ActionResult AlgumMetodo() { }
```

---

## Categoria: Banco de Dados

### P29: Qual é o tamanho máximo da coluna senha agora?
**R:** `varchar(256)`. Anteriormente era `varchar(30)`.

### P30: Por que aumentar de 30 para 256?
**R:** Hashes gerados pelo PasswordHasher ocupam ~200 caracteres. 30 caracteres não é suficiente.

### P31: Posso fazer backup das senhas antes de migrar?
**R:** Sim, e é recomendado:
```sql
-- Backup da tabela
SELECT * INTO pessoas_backup FROM pessoas;
```

### P32: Como verificar se uma senha específica foi migrada?
**R:** 
```sql
SELECT cpf, LEN(senha) as tamanho FROM pessoas WHERE cpf = '123.456.789-00';

-- Se tamanho > 100, foi migrada para hash
-- Se tamanho < 50, ainda está em texto puro
```

### P33: Posso restaurar as senhas antigas?
**R:** Não. Hashes são irreversíveis. Você precisará gerar novos hashes ou resetar senhas.

---

## Categoria: Fluxo

### P34: Qual é o fluxo completo do login?
**R:**
1. Usuário acessa GET /Auth/Login
2. Preenche CPF e Senha
3. Submete POST /Auth/Login
4. AuthController valida campos
5. Repository busca CPF no banco
6. PasswordHasher valida senha
7. Se válido: SignInAsync() → Cookie criado
8. Se inválido: Erro exibido
9. Com cookie: [Authorize] permite acesso

### P35: O que acontece após 30 minutos de inatividade?
**R:**
1. Cookie expira
2. Na próxima requisição, [Authorize] não consegue validar
3. Usuário é redirecionado para /Auth/Login
4. Precisa fazer login novamente

### P36: Posso fazer login em múltiplos navegadores ao mesmo tempo?
**R:** Sim. Cada navegador recebe seu próprio cookie independente.

### P37: O que acontece se alguém tenta acessar uma rota protegida sem cookie?
**R:**
1. [Authorize] verifica se tem cookie válido
2. Se não tiver: Redireciona para /Auth/Login
3. Se estiver expirado: Redireciona para /Auth/Login

---

## Categoria: Funcionalidade Existente

### P38: A implementação de autenticação afeta os CRUDs existentes?
**R:** Não. Apenas o `FuncionarioController` foi protegido. `ClienteController`, `ReservaController`, etc., continuam funcionando normalmente (sem autenticação).

### P39: Os clientes conseguem fazer login como funcionários?
**R:** Não. O CPF de cliente não existe em `funcionarios`, então o login falha.

### P40: Posso cadastrar novos funcionários sem estar autenticado?
**R:** Não. GET e POST para `/Funcionario/Create` requerem autenticação.

---

## Categoria: Desenvolvimento

### P41: Como adicionar novas informações ao login?
**R:** Adicione mais um Claim:
```csharp
claims.Add(new Claim("telefone", funcionario.Telefone));
```

### P42: Como criar um formulário de reset de senha?
**R:** 
1. Criar nova view `Views/Auth/ResetPassword.cshtml`
2. Adicionar action `ResetPassword()` em `AuthController`
3. Gerar novo hash com `PasswordHasherUtil.HashPassword()`
4. Atualizar senha no banco

### P43: Como integrar autenticação com outros controllers?
**R:** Adicione o atributo `[Authorize]` em qualquer classe:
```csharp
[Authorize(AuthenticationSchemes = "FuncionarioScheme")]
public class MeuController : Controller { }
```

### P44: Como testar autenticação em testes unitários?
**R:** Use `Mock<IAuthRepository>` e simule o retorno de `Funcionario`.

### P45: Como debugar problemas de autenticação?
**R:**
```csharp
// Em AuthController
if (funcionario == null) {
    Console.WriteLine($"Falha ao autenticar CPF: {cpf}");
}
```

---

## Categoria: Produção

### P46: Preciso fazer algo especial antes de colocar em produção?
**R:**
1. Executar migração de senhas no banco
2. Testar login com dados reais
3. Usar HTTPS
4. Fazer backup do banco

### P47: Como garantir que as sessões sejam seguras em produção?
**R:**
- Use HTTPS (obrigatório)
- Configure `cookie.SecurePolicy = Always`
- Configure `cookie.HttpOnly = true`
- Configure `cookie.SameSite = Lax`

### P48: Quantos usuários simultâneos o sistema suporta?
**R:** Ilimitado. O ASP.NET Core MVC é escalável. O limite depende do servidor SQL e da máquina hospedeira.

### P49: Como monitorar tentativas de login falhadas?
**R:** Adicione logging:
```csharp
if (funcionario == null) {
    _logger.LogWarning($"Login falhou para CPF: {cpf}");
}
```

### P50: Como resetar a senha de um usuário em produção?
**R:**
1. Contactar administrador
2. Gerar novo hash
3. Atualizar no banco de dados

---

## Resumo Rápido

| Pergunta | Resposta |
|----------|----------|
| Como faço login? | Via CPF e Senha em /Auth/Login |
| E se esquecer? | Contacte administrador |
| Quanto tempo dura a sessão? | 30 minutos de inatividade |
| Senhas são seguras? | Sim, com hash + salt |
| Clientes conseguem acessar? | Não, apenas funcionários |
| Preciso fazer something antes? | Sim, migrar senhas |
| Como saber qual usuário está logado? | Via `User.FindFirst()` |
| Posso adicionar mais rotas protegidas? | Sim, com `[Authorize]` |

---

**Data:** 2026-11-02  
**Versão:** 1.0  
**Status:** Pronto para consulta
