# Relatório – Reserva com várias mesas (N:N), views SQL, botão Sair e testes

Data: 04/11/2026 (horário de Brasília)

## 1. Banco de dados x script.sql

- Comparei o `script.sql` novo com o banco real **gestaoReserva** (só leitura via `sqlcmd`).
- **O banco já está com a estrutura nova** (recriado em 04/11/2026 às 17:13):
  - `pessoas` sem a coluna `senha`;
  - `reservas` sem a coluna `mesas_nr`;
  - mesas da reserva só na tabela `Reservas_Mesas` (N:N);
  - views `v_clientes`, `v_ambientes`, `v_mesas`, `v_reservas`, `v_reservas_mesas` e `v_reservas_detalhadas` criadas.
- **Nada foi alterado na estrutura do banco e o script não foi executado.**
- **A view `v_funcionarios` NÃO existe no banco**: no script, as linhas `1. Visualização...`, `2. ...` até `6. ...` estão sem `--`, então esse bloco dá erro de sintaxe. Por isso o código não usa essa view (ver problemas).
- O código antigo ainda gravava/lia `pessoas.senha` e `reservas.mesas_nr`, colunas que não existem mais. Ele foi ajustado para seguir o script novo.

## 2. O que foi implementado

### Reserva com mais de uma mesa
- **Modelo `Reserva`**: o campo `MesaId` saiu. Entraram:
  - `MesaIds`: lista de mesas;
  - `MesasDescricao`: texto de cada mesa, por exemplo "Mesa 3 · Externo · 4 lugares".
- **Tela Nova reserva (mapa)**: o cliente marca e desmarca **várias mesas livres**, inclusive de ambientes diferentes.
  - Cada mesa vira um campo escondido `MesaIds`.
  - Um resumo mostra as mesas escolhidas.
  - Ao trocar o dia, mesas que deixaram de estar livres são retiradas da seleção.
  - Mesa ocupada ou reservada no dia não pode ser clicada.
- **Validação no servidor** (`ReservaController.Create`):
  - exige ao menos uma mesa;
  - confere se cada mesa existe;
  - confere se **todas** estão livres no dia;
  - se uma mesa falhar, **nada é gravado**.
- **Gravação** (`ReservaSqlRepository.Create`): em uma única transação.
  - Primeiro confere de novo, com bloqueio, se alguma mesa já tem reserva ativa no dia (evita duas reservas ao mesmo tempo).
  - Depois grava em `reservas` e uma linha em `Reservas_Mesas` para cada mesa.
- **"Mesa reservada no dia"** (mapa e validações) agora vem da tabela N:N, lida pela view `v_reservas_mesas`.
- **Edição pelo cliente**:
  - continua alterando só o dia e a quantidade de pessoas;
  - as mesas continuam as originais;
  - o servidor confere se **todas** as mesas da reserva estão livres no novo dia.
- **Telas que mostram todas as mesas**: Minhas reservas, Detalhes, Editar e Cancelar do cliente, e Reservas, Detalhes e Editar do restaurante.
- **`MesaReservaSync`** (confirmar ou cancelar muda o status da mesa): agora vale para todas as mesas da reserva.
- **Exclusão de mesa** (`MesaSqlRepository.Delete`): a checagem de reserva ativa passou a usar `Reservas_Mesas`, porque `mesas_nr` não existe mais.

### Ajustes para o script novo (sem coluna `senha` em `pessoas`)
- `ClienteSqlRepository`, `FuncionarioSqlRepository` e `PessoaSqlRepository` não leem nem gravam mais `pessoas.senha`.
- As senhas continuam só em `clientes.senha_hash` e `funcionarios.senha_hash`.

### Testes automatizados
- `MemoryRepositories.cs` e `FluxosTests.cs` foram ajustados para `MesaIds`.
- 4 testes novos:
  - reserva com várias mesas + bloqueio de mesa já reservada;
  - edição bloqueada quando a mesa está ocupada no novo dia;
  - Sair do header do cliente;
  - Sair do header do funcionário.

## 3. Views SQL reaproveitadas

| View | Onde foi usada |
|---|---|
| `v_clientes` | `ClienteSqlRepository.Read()` e `Read(id)` (a lista e os detalhes de clientes; o hash da senha não sai do banco) |
| `v_ambientes` | `AmbienteSqlRepository.Read()` e `Read(id)` |
| `v_reservas` | base da lista e dos detalhes de reservas (número, quantidade, data e status em texto) |
| `v_reservas_mesas` | mesas de cada reserva (N:N) e checagem de "mesa reservada no dia" |
| `v_mesas` | número, ambiente e lugares de cada mesa exibida na reserva |

Observações:
- `v_funcionarios` não foi usada porque não existe no banco (erro no script).
- `v_reservas_detalhadas` não foi usada porque não traz o código da mesa nem o id do cliente, e o sistema precisa dos dois para filtrar e validar.

## 4. Botão "Sair" do header

- **Causa:** o formulário do Sair é gerado numa página já logada, então o token antifalsificação fica ligado ao usuário (cliente ou funcionário). As ações `Auth/Logout` e `Auth/LogoutCliente` são anônimas (`[AllowAnonymous]`, sem esquema). Na hora de validar, o usuário aparecia vazio, o token não batia e o servidor respondia **400 (Bad Request)**. O usuário continuava logado.
- O rodapé "Sair" do painel do restaurante tinha o mesmo problema.
- O "Sair da conta" da Home funcionava, porque a Home é anônima.
- **Correção** (`AuthController`): `[IgnoreAntiforgeryToken]` só em `Logout` e `LogoutCliente`. Sair só apaga o cookie de login.
- **Confirmado:** testes automatizados e de ponta a ponta. Depois do Sair, o cliente e o funcionário voltam a ser mandados para o login.

## 5. Testes executados

- `dotnet build`: **0 erros**. Os 3 avisos já existiam antes.
- `dotnet test`: **28/28 aprovados** (24 antigos + 4 novos).
- Os dois rodaram numa cópia separada (`C:\Users\isabe\gr_build`), porque o `dotnet watch` estava rodando.

### Ponta a ponta

- O app rodou na porta **5199**, apontado para o **banco real**, que já tem a estrutura nova.
- Todos os dados foram de teste: CPFs `900.000.000-xx`, ambiente "E2E Teste" e mesas 90/91.
- Esses dados foram **apagados no final**. Os contadores de identidade (`pessoas` 8 e `reservas` 4) e o status das mesas (mesa 1 `Ocupada` e o resto `Disponivel`) voltaram ao que eram antes.

| Cenário | Resultado |
|---|---|
| Páginas públicas (/, Login, LoginCliente, Cadastro) | OK (200) |
| Página protegida sem login → login | OK |
| Login do funcionário (senha certa / errada) | OK / recusado |
| Cadastro de funcionário (válido / campos vazios) | OK / recusado |
| Cadastro de cliente 1 e 2 / CPF repetido | OK / recusado |
| Senha do cliente gravada como hash em `clientes.senha_hash` | OK |
| Login de cliente 1 e 2 | OK |
| Cliente 1 com a senha do cliente 2 | recusado (OK) |
| Cliente tentando entrar como funcionário / funcionário como cliente | recusado (OK) |
| Login do funcionário cadastrado pelo app | OK |
| Cliente acessando a área do restaurante | redirecionado (OK) |
| Cadastro e edição de ambiente / capacidade 0 | OK / recusado |
| Cadastro e edição de mesa / 0 lugares | OK / recusado |
| Reserva com 1 mesa | OK |
| Reserva com 3 mesas (2 ambientes) → 3 linhas em `Reservas_Mesas` | OK |
| Detalhes, lista do cliente e lista/detalhes do restaurante mostram todas as mesas | OK |
| Bloqueio: mesa já reservada no dia | OK |
| Bloqueio: uma das várias mesas já reservada (nada gravado) | OK |
| Bloqueio: mesa com status Ocupada | OK |
| Mesma mesa em outro dia | permitido (OK) |
| Validações: data passada, sem mesa, 0 pessoas, mesa inexistente | recusadas (OK) |
| Mapa do dia (`MesasDisponiveis`) marca as 4 mesas reservadas | OK |
| Cliente edita dia/quantidade (mesas e status não mudam mesmo enviando outros valores) | OK |
| Edição bloqueada quando a mesa está ocupada no novo dia | OK |
| Restaurante confirma (só o status muda; as mesas viram Reservada) / status inválido | OK / recusado |
| Cliente cancela (status 2; as mesas voltam a Disponivel e ficam livres no mapa) | OK |
| Reserva cancelada não pode ser editada | OK (400) |
| Excluir mesa com reserva ativa / ambiente com mesas | recusado (OK) |
| Excluir mesa só com reservas canceladas | OK |
| Funcionário edita cliente (senha em branco mantém a antiga) / edita funcionário | OK |
| Excluir funcionário | OK |
| Excluir cliente com reservas | recusado com mensagem; o cliente continua (comportamento atual) |
| Sair do header (cliente e funcionário) | OK (302 e depois pede login) |
| Sair do rodapé do painel / "Sair da conta" da Home | OK |
| Cliente abrindo uma reserva de outro cliente | **ERRO 500** (problema 6 abaixo; não corrigido, fora do escopo) |

## 6. Problemas encontrados (NÃO corrigidos — propostas)

1. **`script.sql`, linhas 214, 231, 248, 262, 278 e 300** – os títulos `1. Visualização ...` estão sem `--`. Isso gera erro de sintaxe e a view `v_funcionarios` não é criada.
   *Proposta:* trocar por `-- 1. Visualização ...` e assim por diante.
2. **`script.sql`, inserts de `clientes` e `funcionarios`** – todos com `senha_hash = NULL`. Nenhum cliente nem funcionário do banco atual consegue fazer login.
   *Proposta:* inserir hashes do PasswordHasher (ex.: os de `Func@123` e `Cliente@123` usados antes) ou criar uma tela para definir a senha.
3. **Ambiente "Salão Interno" no banco x mapa** (`ReservaController.cs`, `NomesAmbientesMapa = ["Interno", "Externo", "Sacada"]`) – o mapa procura o nome exato "Interno". Com o banco atual, a opção Interno aparece desabilitada ("cadastre em Ambientes").
   *Proposta:* renomear o ambiente para "Interno" ou comparar por `Contains("Interno")` / pelo código.
4. **`MesaSqlRepository.Create`** – `cmd.Parameters.AddWithValue("@Id", mesa.Numero)`: a chave `nr` é igual ao número. Não dá para cadastrar a "Mesa 5" em outro ambiente (conflito de PK com a mesa nr 5).
   *Proposta:* gerar `nr` com `MAX(nr)+1`, como já é feito no ambiente.
5. **`MesaController` e `AmbienteController` sem `[Authorize]`** – qualquer pessoa, sem login, consegue abrir e enviar cadastro, edição e exclusão de mesas e ambientes.
   *Proposta:* `[Authorize(AuthenticationSchemes = "FuncionarioScheme")]` nas duas classes.
6. **`ReservaController.GarantirReservaDoCliente`** – `return Forbid();` sem esquema. Quando um cliente abre a reserva de outro, dá **erro 500**, porque não há esquema padrão.
   *Proposta:* `return Forbid("ClienteScheme");` ou `return NotFound();`.
7. **Status "Reservada" da mesa é global** (`MesaReservaSync`) – confirmar uma reserva para um dia marca a mesa como Reservada para sempre, até cancelar. A disponibilidade por dia já vem de `Reservas_Mesas`, então os dois conceitos se misturam.
   *Proposta:* mostrar a situação por dia só a partir das reservas e deixar o status da mesa apenas para Livre/Ocupada.
8. **`MesaSqlRepository.Delete`** – ao excluir uma mesa que só tem reservas canceladas, os vínculos dela em `Reservas_Mesas` são apagados, e o histórico dessas reservas perde a mesa.
   *Proposta:* não excluir mesas com histórico (só desativar) ou aceitar a perda.
9. **Procedures do script** (`InsereReserva`, `InsereReservaMesa`, `ListDisp`...) não são usadas pela aplicação. Não é erro, só fica registrado.

## 7. Limitações

- O mapa com seleção múltipla (JavaScript) não foi clicado num navegador real. Foi validado o que ele envia (vários `MesaIds`) e todas as regras no servidor, pelos testes automatizados e de ponta a ponta.
- No Sair, o token antifalsificação deixou de ser exigido (só nas duas ações de sair). O risco é alguém de fora conseguir deslogar o usuário, o que é considerado baixo.
- O cliente não troca as mesas de uma reserva já feita (regra mantida). Para mudar de mesa, ele cancela e faz outra reserva.
- Backups dos arquivos alterados: `*.bak_v11`, ao lado de cada arquivo.

---

# Atualização (04/11/2026, horário de Brasília) – itens 2 e 5 corrigidos

## Item 2 – senhas iniciais no `script.sql`
- Nos inserts de `clientes` e `funcionarios`, o `senha_hash` deixou de ser `NULL` e passou a ter hashes válidos, gerados com o `PasswordHasherUtil` do próprio projeto (cada usuário com o seu hash).
- **Onde estão as senhas iniciais:** num comentário logo acima de cada insert no `script.sql` ("-- Senha inicial de todos os clientes" e "-- Senha inicial de todos os funcionários").
- **Validação:**
  - Rodei o trecho de tabelas e dados iniciais num banco temporário (`gestaoReserva_validacao_senhas`): **0 erros**, 3 clientes e 3 funcionários com hash.
  - Com o app apontado para esse banco, os 3 clientes e os 3 funcionários entraram (302). Senha trocada entre cliente e funcionário foi recusada (200).
  - O banco temporário foi apagado. **O banco real não foi alterado.**
- **Banco real hoje:** 3 de 3 clientes e 3 de 4 funcionários estão **sem `senha_hash`** e não conseguem entrar. O outro funcionário tem um valor que não é hash e só funciona pela comparação de texto simples.
- **Forma mais simples de liberar (não aplicada):** um `UPDATE` copiando os hashes do `script.sql` para quem está sem senha. Por exemplo: `UPDATE clientes SET senha_hash = '<hash de cliente do script>' WHERE senha_hash IS NULL;` e o mesmo para `funcionarios`. Depois cada um troca a senha pela tela de edição. Recriar o banco com o script também resolve, mas apaga os dados atuais.

## Item 5 – login obrigatório em Mesas e Ambientes
- `MesaController` e `AmbienteController` agora têm `[Authorize(AuthenticationSchemes = "FuncionarioScheme")]`, o mesmo padrão do `FuncionarioController` e do `AreaRestauranteController`.
- O mapa da reserva (`/Reserva/MesasDisponiveis`) fica no `ReservaController` e não foi mexido.
- **Teste com curl** (app numa porta separada, banco temporário):

| Cenário | Resultado |
|---|---|
| Sem login: /Mesa/Index, /Mesa/Create, /Ambiente/Index, /Ambiente/Create | 302 → /Auth/Login (OK) |
| Logado como cliente: /Mesa/Index, /Mesa/Create, /Ambiente/Index | 302 → /Auth/Login (OK, não acessa) |
| Logado como cliente: /Reserva/Create e mapa do dia | 200; mapa com 40 mesas, 2 reservadas no dia (OK) |
| Logado como funcionário: telas de Mesa e Ambiente | 200 (OK) |

- **Testes automatizados:** ajustados (agora fazem login de funcionário para Mesa e Ambiente) e foram criados 3 novos. **`dotnet test`: 31/31 aprovados**, rodado numa cópia separada.
- **Observação:** o `script.sql` rodado inteiro ainda só cria a view `v_reservas_detalhadas`, por causa do item 1 (títulos sem `--`), que não foi aprovado para correção. No teste, as views foram copiadas do banco real para o banco temporário.
- Backups: `*.bak_v12` ao lado de cada arquivo alterado.