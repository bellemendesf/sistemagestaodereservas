# Validação da versão final

## Executado

- Compilação real com SDK .NET 10.0.402: sucesso, 0 erros, 4 avisos de auditoria NuGet (dois avisos repetidos no restore/build).
- Inicialização real do ASP.NET Core: sucesso.
- Compilação Razor das Views: incluída no build, sem erros.
- Comparação binária: as 30 Views e os 9 arquivos de wwwroot são idênticos aos originais da versão A.
- Revisão dos 31 comandos SQL dos repositórios: os parâmetros utilizados correspondem aos parâmetros fornecidos ou às variáveis locais declaradas.
- Controllers: nenhuma assinatura pública de ação duplicada.

## Requisições HTTP executadas

As páginas abaixo foram servidas pela versão final, configurada com os repositórios SQL. Os GETs de criação não abrem conexão com o banco.

| Rota | Resultado |
| --- | --- |
| `/` | HTTP 200 |
| `/AreaCliente` | HTTP 200 |
| `/AreaRestaurante` | HTTP 200 |
| `/Cliente/Create` | HTTP 200 |
| `/Funcionario/Create` | HTTP 200 |
| `/Ambiente/Create` | HTTP 200 |
| `/Mesa/Create` | HTTP 200 |
| `/Reserva/Create` | HTTP 200 |

## Avisos reais do build

A dependência transitiva `System.Security.Cryptography.Pkcs` 9.0.18 gerou NU1902 (moderado) e NU1903 (alto), associados aos avisos GHSA-2fmv-rv2c-59x7 e GHSA-846f-f764-76mx. As versões de bibliotecas fornecidas em B foram mantidas conforme o escopo. Os avisos não impedem compilar, mas não foram corrigidos neste pacote.

## Não executado neste ambiente

Não há uma instância do SQL Server disponível. Portanto, o script e as operações CRUD no banco foram revisados, mas não executados. O sucesso do build e dos GETs não comprova a execução dos comandos SQL.

## Conferência na instalação local

1. Execute o script em uma instalação nova e configure a conexão.
2. Cadastre, consulte e edite um ambiente, uma mesa, um funcionário e um cliente.
3. Edite um cliente e um funcionário deixando a senha vazia; confira que a senha anterior foi mantida.
4. Crie uma reserva para um CPF já cadastrado e outra para um cliente novo. Confira as relações em pessoas, clientes, reservas e Reservas_Mesas.
5. Edite a reserva pelo fluxo do cliente; cliente, funcionário e status devem permanecer inalterados.
6. Na área do restaurante, altere o status; os demais campos devem permanecer inalterados.
7. Cancele a reserva; ela deve continuar na listagem com status Cancelada e rejeitar edição ou reativação.
8. Envie uma reserva com mesa ou funcionário inexistente; a gravação deve falhar sem deixar uma nova pessoa/cliente/reserva parcialmente gravados.
9. Tente excluir uma mesa, cliente ou funcionário já vinculado; o banco deve rejeitar a exclusão.
10. Reinicie o aplicativo e confira a persistência dos dados.
