# Sistema de Gestão de Reservas — versão final

Integração de `sistemagestaodereservas-testes (1)(1).zip` (A) e `sistemagestaodereservas-feature-database(1).zip` (B), com as correções de métodos e SQL autorizadas após a análise.

## Executar

1. Instale o SDK .NET 10 e tenha uma instância do SQL Server disponível.
2. Para uma instalação nova, execute `script.sql` no SQL Server Management Studio. Ele cria o banco `gestaoReserva` e suas tabelas. Não é um script de migração: não o execute sobre um banco já existente nem apague dados para aplicá-lo.
3. Em `appsettings.json`, ajuste `ConnectionStrings:DefaultConnection` para sua instância. O endereço herdado é `localhost\\MSSQLSERVER04`; o catálogo foi corrigido para `gestaoReserva`. A conexão usa autenticação integrada do Windows.
4. Abra o terminal na pasta que contém `GestaoReservas.csproj` e execute:

```powershell
dotnet restore
dotnet build
dotnet run --launch-profile http
```

5. Acesse `http://localhost:5004`.

O pacote Microsoft.Data.SqlClient 7.1.1 e o alvo net10.0 foram mantidos da versão B. Os repositórios em memória de A continuam no código, mas o Program.cs registra os repositórios SQL.

## Primeiro uso

- Cadastre um funcionário e anote seu ID na listagem.
- Cadastre uma mesa e anote seu número. A chave da mesa no banco é esse número.
- Ao criar uma reserva, informe nome, CPF, telefone, número da mesa cadastrada, ID do funcionário cadastrado, data/hora e quantidade de pessoas.
- Se o CPF já existir, a reserva reutiliza a pessoa e o cliente cadastrados, sem sobrescrever seus dados. Caso contrário, o cadastro de cliente é criado com os dados do formulário; a senha fica vazia, pois essa tela não solicita senha. A edição de Cliente permite defini-la depois.
- CPF e telefone continuam sujeitos às restrições de unicidade do script original. Para repetir uma reserva do mesmo cliente, utilize o mesmo CPF.
- Use a área do restaurante para confirmar ou cancelar uma reserva. Na área do cliente, editar uma reserva preserva seu status, cliente e funcionário.

## O que foi integrado

- Controllers, rotas de áreas, Views e regras de cancelamento da versão A.
- Configuração de conexão, SqlClient, injeção AddScoped e estrutura de repositórios SQL da versão B.
- `Reserva` mantém `Cliente` para as Views e `ClienteId` para o vínculo persistido.
- Consultas e parâmetros usam os nomes reais de `script.sql`: pessoas, clientes, funcionarios, ambientes, mesas, reservas e Reservas_Mesas.
- DataHora é gravada nas colunas dia/horario. Os estados Pendente, Confirmada e Cancelada correspondem a 0, 1 e 2 no banco.
- A criação e a edição de registros relacionados usam transações SQL; o fechamento da conexão ocorre também em caso de falha.
- A exclusão de reserva apenas marca status 2. Reservas canceladas não são editadas nem reativadas.
- Senha vazia na edição de pessoas conserva a senha armazenada.
- Foram resolvidas as assinaturas Delete duplicadas de Cliente e Funcionario.
- Comentários `// UNIÃO:` identificam os pontos de integração. Os comentários originais foram preservados nos arquivos reaproveitados.

## Ajustes no script

A estrutura relacional de B foi mantida, com três adaptações para as telas de A:

- `ambientes.codigo` e `reservas.numero` passaram a usar IDENTITY, porque os formulários não fornecem esses identificadores.
- `mesas.codigo_ambientes` passou a permitir NULL, pois a tela de mesa não possui campo para escolher um ambiente. Nenhum ambiente é escolhido automaticamente. Uma associação já cadastrada é preservada na edição da mesa.
- Separadores entre constraints foram corrigidos.

Para um banco já existente, esses ajustes exigem uma migração específica; o pacote contém somente o script de criação de uma instalação nova.

## Comportamentos e limites preservados

As 30 Views e todos os arquivos de wwwroot foram preservados byte a byte. Isso inclui textos antigos que mencionam armazenamento em memória e marcadores de Markdown já presentes em algumas Views.

O banco impede excluir clientes, funcionários e mesas vinculados a reservas, inclusive canceladas, preservando o histórico. Também impede renumerar uma mesa já referenciada. Os POSTs retornam uma mensagem HTTP 400 quando uma operação viola as restrições ou falha no SQL.

O projeto original não implementa autenticação nem autorização por usuário. A separação das áreas representa os fluxos e regras dos controllers, não controle de acesso. O tratamento de senhas segue o código original.

## Validação

Consulte `VALIDACAO.md` para os resultados executados neste ambiente e os testes que dependem do seu SQL Server.
