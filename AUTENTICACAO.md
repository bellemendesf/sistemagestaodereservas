# Autenticação — Gestão de Reservas

Este documento descreve o fluxo de login de **cliente** e **funcionário**, os termos usados e por que cada peça foi escolhida. A ideia segue o exemplo **TaskWeb** (formulário simples → repositório valida → guarda quem está logado → redireciona), adaptada a este projeto.

## Referência TaskWeb (Downloads)

No TaskWeb:

1. `UsuarioController.Login` (GET) mostra o formulário.
2. POST envia `LoginViewModel` (e-mail + senha).
3. `IUsuarioRepository.Login(model)` consulta o banco e devolve o usuário ou `null`.
4. Se ok: grava `UsuarioId` e `Nome` na **sessão** e redireciona para `Tarefa/Index`.
5. `TarefaController` lê `HttpContext.Session.GetInt32("UsuarioId")` para listar só as tarefas daquele usuário.

Aqui fazemos o mesmo **fluxo lógico**, trocando sessão por **cookie de autenticação** (necessário porque existem **dois tipos de usuário** no mesmo site) e usando **CPF** em vez de e-mail.

---

## Fluxo do cliente

| Passo | O que acontece |
|--------|----------------|
| 1 | Usuário abre `/Auth/LoginCliente` ou vem do cadastro em `Cliente/Create` (redireciona para o login). |
| 2 | POST com `LoginViewModel` (`Cpf`, `Senha`) + token antifalsificação. |
| 3 | `AuthSqlRepository.LoginCliente(model)` busca `pessoas` + `clientes` pelo CPF e confere a senha (hash ou legado em texto, como no funcionário). |
| 4 | Se inválido: mesma tela com `ViewBag.Error` (como no TaskWeb). |
| 5 | Se válido: `SignInAsync` no esquema **`ClienteScheme`**, cookie `ClienteAuth`, claims com id, nome e role `Cliente`. |
| 6 | Redireciona para `Reserva/Index` (Minhas reservas). `/AreaCliente` também redireciona para lá. |
| 7 | `ReservaController` exige `[Authorize(ClienteScheme)]`. Sem cookie, o middleware manda para `/Auth/LoginCliente`. |
| 8 | Em reservas: `ClienteLogadoId()` lê o id da claim (equivalente ao `UsuarioId` da sessão no TaskWeb). Listagem e CRUD só dos dados desse cliente. |
| 9 | Sair: POST `Auth/LogoutCliente` → `SignOutAsync` → Home. |

**Cadastro:** `Cliente/Create` continua **anônimo** (`[AllowAnonymous]`). Quem gerencia todos os clientes (listagem, editar, excluir) é o **funcionário** logado.

---

## Fluxo do funcionário

| Passo | O que acontece |
|--------|----------------|
| 1 | Usuário abre `/Auth/Login` (view `LoginFuncionario`). |
| 2 | POST com o mesmo `LoginViewModel`. |
| 3 | `IAuthRepository.Login(model)` valida em `pessoas` + `funcionarios`. |
| 4 | Erro: `ViewBag.Error`. Sucesso: `SignInAsync` em **`FuncionarioScheme`**, cookie `FuncionarioAuth`. |
| 5 | Redireciona para `AreaRestaurante/Index`. |
| 6 | Controllers da equipe (`FuncionarioController`, `AreaRestauranteController`, CRUD administrativo de `Cliente`, etc.) usam `[Authorize(FuncionarioScheme)]`. |
| 7 | Sair: POST `Auth/Logout`. |

Novos funcionários são cadastrados pela área do restaurante (`Funcionario/Create`), não pela Home — alinhado ao que já estava definido no projeto.

---

## Termos

| Termo | Significado |
|--------|-------------|
| **Esquema de autenticação** | Nome do “tipo” de login (`FuncionarioScheme` / `ClienteScheme`). Permite dois cookies ao mesmo tempo no navegador, um por perfil. |
| **Cookie de autenticação** | O ASP.NET Core guarda o ticket criptografado no cookie (`FuncionarioAuth` / `ClienteAuth`). Substitui a **sessão** do TaskWeb, com expiração (5 min, renovação deslizante). |
| **Claim** | Pedacinho de informação no ticket (ex.: `NameIdentifier` = id da pessoa, `Name` = nome). |
| **`LoginViewModel`** | Objeto do formulário (`Cpf`, `Senha`), igual ao `LoginViewModel` do TaskWeb (lá: e-mail + senha). |
| **`IAuthRepository`** | Única porta para “credenciais → usuário ou null”, como `IUsuarioRepository.Login` no TaskWeb. |
| **Antifalsificação (CSRF)** | Token obrigatório em POSTs neste projeto (`AutoValidateAntiforgeryToken`). O TaskWeb da faculdade não usa; aqui mantemos por segurança. |
| **`[Authorize]`** | Declara rotas protegidas. No TaskWeb a proteção é manual (ler sessão); aqui o framework redireciona quem não está logado. |

---

## Por que não copiamos a sessão do TaskWeb?

- O TaskWeb tem **um** perfil (usuário de tarefas).
- Este sistema tem **cliente** e **funcionário** com áreas diferentes. Dois esquemas de **cookie** deixam os logins independentes (cliente pode estar logado sem o funcionário, e vice-versa).
- Cookies + `[Authorize]` são o padrão já usado no login de funcionário deste repositório e integram bem com `UseAuthentication` / `UseAuthorization` em `Program.cs`.

---

## Arquivos principais

- `Program.cs` — registra os dois cookies e `IAuthRepository`.
- `Controllers/AuthController.cs` — login/logout (espelha `UsuarioController` do TaskWeb).
- `Models/LoginViewModel.cs` — dados do formulário.
- `Repositories/AuthSqlRepository.cs` — SQL + verificação de senha.
- `Views/Auth/LoginCliente.cshtml` e `LoginFuncionario.cshtml` — formulário + `ViewBag.Error`.
- `Controllers/ReservaController.cs` — área do cliente logado (filtro por id).
- `Controllers/AreaRestauranteController.cs` / `FuncionarioController` — área do funcionário.

---

## Teste rápido manual

1. Cliente: cadastro → login em `/Auth/LoginCliente` → Área do Cliente → nova reserva / minhas reservas.
2. Funcionário: login em `/Auth/Login` → Área Restaurante → confirmar reservas, mesas, etc.
3. Seed (se usar `script.sql`): cliente de teste com senha definida no script; funcionário idem.

Testes automatizados: `dotnet test tests/GestaoReservas.Tests/GestaoReservas.Tests.csproj`.
