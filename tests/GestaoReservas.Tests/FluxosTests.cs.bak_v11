using System.Net;
using System.Text.RegularExpressions;
using GestaoReservas.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace GestaoReservas.Tests;

// Cada teste usa seu próprio servidor e seus próprios dados.
public class AplicacaoTeste : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IAmbienteRepository>();
            services.RemoveAll<IMesaRepository>();
            services.RemoveAll<IClienteRepository>();
            services.RemoveAll<IFuncionarioRepository>();
            services.RemoveAll<IReservaRepository>();
            services.RemoveAll<IAuthRepository>();
            services.AddSingleton<IAmbienteRepository, AmbienteMemoryRepository>();
            services.AddSingleton<IMesaRepository, MesaMemoryRepository>();
            services.AddSingleton<IClienteRepository, ClienteMemoryRepository>();
            services.AddSingleton<IFuncionarioRepository, FuncionarioMemoryRepository>();
            services.AddSingleton<IReservaRepository, ReservaMemoryRepository>();
            services.AddSingleton<IAuthRepository, AuthMemoryRepository>();
        });
    }

    public HttpClient Cliente() => CreateClient(new WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = false
    });
}

public class FluxosTests
{
    private static async Task<HttpResponseMessage> Post(HttpClient client, string get, string post,
        Dictionary<string, string> fields)
    {
        var page = await client.GetAsync(get);
        page.EnsureSuccessStatusCode();
        var html = await page.Content.ReadAsStringAsync();
        var token = Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
        Assert.True(token.Success, "O formulário deve incluir token antifalsificação.");
        fields["__RequestVerificationToken"] = WebUtility.HtmlDecode(token.Groups[1].Value);
        return await client.PostAsync(post, new FormUrlEncodedContent(fields));
    }

    private static Dictionary<string, string> DadosReserva() => new()
    {
        ["Cliente.Nome"] = "Cliente de teste", ["Cliente.Cpf"] = "12345678901",
        ["Cliente.Telefone"] = "17999999999", ["Cliente.EmailPessoal"] = "",
        ["MesaId"] = "1", ["FuncionarioID"] = "1", ["DataHora"] = "2027-01-02",
        ["QtdPessoas"] = "2", ["Status"] = "Confirmada"
    };

    private static async Task LoginClienteTeste(HttpClient client, IServiceProvider? services = null)
    {
        if (services != null && !services.GetRequiredService<IClienteRepository>().Read().Any(c => c.Cpf == "12345678901"))
        {
            await Post(client, "/Cliente/Create", "/Cliente/Create", new Dictionary<string, string>
        {
            ["Nome"] = "Cliente de teste",
            ["Cpf"] = "12345678901",
            ["Telefone"] = "17999999999",
            ["EmailPessoal"] = "teste@email.com",
            ["Senha"] = "Cliente@123"
            });
        }
        else if (services == null)
        {
            await Post(client, "/Cliente/Create", "/Cliente/Create", new Dictionary<string, string>
            {
                ["Nome"] = "Cliente de teste",
                ["Cpf"] = "12345678901",
                ["Telefone"] = "17999999999",
                ["EmailPessoal"] = "teste@email.com",
                ["Senha"] = "Cliente@123"
            });
        }

        var login = await Post(client, "/Auth/LoginCliente", "/Auth/LoginCliente", new Dictionary<string, string>
        {
            ["cpf"] = "12345678901",
            ["senha"] = "Cliente@123"
        });
        Assert.Equal(HttpStatusCode.Redirect, login.StatusCode);
    }

    private static async Task LoginFuncionarioTeste(HttpClient client, IServiceProvider services)
    {
        if (!services.GetRequiredService<IFuncionarioRepository>().Read().Any())
        {
            services.GetRequiredService<IFuncionarioRepository>().Create(new Funcionario
            {
                Nome = "Funcionario teste",
                Cpf = "111.222.333-44",
                EmailInstitucional = "func@test.com",
                Senha = "Func@123"
            });
        }
        var login = await Post(client, "/Auth/Login", "/Auth/Login", new Dictionary<string, string>
        {
            ["cpf"] = "111.222.333-44",
            ["senha"] = "Func@123"
        });
        Assert.Equal(HttpStatusCode.Redirect, login.StatusCode);
    }

    private static void PrepararMesaTeste(IServiceProvider services)
    {
        services.GetRequiredService<IAmbienteRepository>().Create(new Ambiente
        {
            Nome = "Interno",
            Capacidade = 10,
            Status = "Disponivel"
        });
        services.GetRequiredService<IMesaRepository>().Create(new Mesa
        {
            Numero = 1,
            AmbienteId = 1,
            QtdLugares = 4,
            Status = "Livre",
            AmbienteNome = "Interno"
        });
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/Auth/LoginCliente")]
    [InlineData("/AreaRestaurante")]
    [InlineData("/Cliente/Create")]
    [InlineData("/Funcionario/Create")]
    [InlineData("/Ambiente/Create")]
    [InlineData("/Mesa/Create")]
    [InlineData("/AreaRestaurante/Reservas")]
    public async Task PaginasExistentesAbrem(string route)
    {
        using var app = new AplicacaoTeste();
        using var client = app.Cliente();
        if (route.StartsWith("/AreaRestaurante", StringComparison.Ordinal) || route == "/Funcionario/Create")
            await LoginFuncionarioTeste(client, app.Services);
        var response = await client.GetAsync(route);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain("```", await response.Content.ReadAsStringAsync());
    }

    [Theory]
    [InlineData("Cliente")]
    [InlineData("Funcionario")]
    [InlineData("Ambiente")]
    [InlineData("Mesa")]
    [InlineData("Reserva")]
    public async Task RegistroInexistenteRetorna404(string entity)
    {
        using var app = new AplicacaoTeste();
        using var client = app.Cliente();
        if (entity == "Reserva")
            await LoginClienteTeste(client, app.Services);
        else if (entity is "Cliente" or "Funcionario")
            await LoginFuncionarioTeste(client, app.Services);
        foreach (var action in new[] { "Edit", "Details", "Delete" })
            Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/{entity}/{action}/999")).StatusCode);
    }

    [Fact]
    public async Task EscritaSemTokenEhRejeitada()
    {
        using var app = new AplicacaoTeste();
        PrepararMesaTeste(app.Services);
        using var client = app.Cliente();
        await LoginClienteTeste(client, app.Services);
        var response = await client.PostAsync("/Reserva/Create", new FormUrlEncodedContent(DadosReserva()));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Empty(app.Services.GetRequiredService<IReservaRepository>().Read());
    }

    [Fact]
    public async Task ReservaPreservaIdentidadeStatusEHistorico()
    {
        using var app = new AplicacaoTeste();
        PrepararMesaTeste(app.Services);
        using var client = app.Cliente();
        await LoginClienteTeste(client, app.Services);
        var repo = app.Services.GetRequiredService<IReservaRepository>();
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/Reserva/Create", "/Reserva/Create", DadosReserva())).StatusCode);
        var original = Assert.Single(repo.Read());
        Assert.Equal("Pendente", original.Status);
        Assert.Equal("Cliente de teste", original.Cliente.Nome);
        var id = original.Id;

        var edit = new Dictionary<string, string>
        {
            ["Id"] = "999", ["MesaId"] = "2", ["QtdPessoas"] = "3", ["DataHora"] = "2027-01-03T20:00",
            ["Status"] = "Confirmada", ["FuncionarioID"] = "999", ["ClienteId"] = "999", ["Cliente.Nome"] = "Outro"
        };
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, $"/Reserva/Edit/{id}", $"/Reserva/Edit/{id}", edit)).StatusCode);
        var atual = repo.Read(id)!;
        // O cliente não troca de mesa: continua a mesa original (1).
        Assert.Equal(1, atual.MesaId);
        Assert.Equal(3, atual.QtdPessoas);
        Assert.Equal("Pendente", atual.Status);
        Assert.Equal("Cliente de teste", atual.Cliente.Nome);

        await LoginFuncionarioTeste(client, app.Services);
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, $"/AreaRestaurante/Edit/{id}", $"/AreaRestaurante/Edit/{id}", new()
        { ["Status"] = "Confirmada", ["MesaId"] = "999", ["QtdPessoas"] = "999" })).StatusCode);
        Assert.Equal("Confirmada", repo.Read(id)!.Status);
        Assert.Equal(1, repo.Read(id)!.MesaId);
        Assert.Equal(3, repo.Read(id)!.QtdPessoas);

        await LoginClienteTeste(client, app.Services);
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, $"/Reserva/Delete/{id}", $"/Reserva/Delete/{id}", new() { ["Id"] = id.ToString() })).StatusCode);
        Assert.Equal("Cancelada", Assert.Single(repo.Read()).Status);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync($"/Reserva/Edit/{id}")).StatusCode);

        await LoginFuncionarioTeste(client, app.Services);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync($"/AreaRestaurante/Edit/{id}")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await Post(client, "/AreaRestaurante/Reservas", $"/AreaRestaurante/Edit/{id}", new() { ["Status"] = "Pendente" })).StatusCode);

        await LoginClienteTeste(client, app.Services);
        Assert.Equal(HttpStatusCode.BadRequest, (await Post(client, "/Reserva/Index", $"/Reserva/Edit/{id}", edit)).StatusCode);
        Assert.Equal("Cancelada", repo.Read(id)!.Status);
    }

    [Fact]
    public async Task ReservaInvalidaNaoEhGravadaEReexibeDados()
    {
        using var app = new AplicacaoTeste();
        PrepararMesaTeste(app.Services);
        using var client = app.Cliente();
        await LoginClienteTeste(client, app.Services);
        var fields = DadosReserva();
        fields["QtdPessoas"] = "texto";
        var response = await Post(client, "/Reserva/Create", "/Reserva/Create", fields);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(app.Services.GetRequiredService<IReservaRepository>().Read());
        Assert.Contains("Cliente de teste", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task EdicaoDeReservaInvalidaNaoAlteraRegistro()
    {
        using var app = new AplicacaoTeste();
        PrepararMesaTeste(app.Services);
        using var client = app.Cliente();
        await LoginClienteTeste(client, app.Services);
        await Post(client, "/Reserva/Create", "/Reserva/Create", DadosReserva());
        var fields = DadosReserva();
        fields["QtdPessoas"] = "0";
        fields["DataHora"] = "nao-e-data";
        var response = await Post(client, "/Reserva/Edit/1", "/Reserva/Edit/1", fields);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var stored = app.Services.GetRequiredService<IReservaRepository>().Read(1)!;
        Assert.Equal(2, stored.QtdPessoas);
        // Reserva por dia: guarda só a data, sem horário.
        Assert.Equal(new DateTime(2027, 1, 2), stored.DataHora);
    }

    [Theory]
    [InlineData("Cliente")]
    [InlineData("Funcionario")]
    public async Task CadastroEdicaoSenhaEmBrancoEExclusao(string entity)
    {
        using var app = new AplicacaoTeste();
        using var client = app.Cliente();
        Dictionary<string, string> fields = new()
        {
            ["Nome"] = "Pessoa", ["Cpf"] = "12345678901", ["EmailPessoal"] = "pessoa@example.com",
            ["Senha"] = "senha-original", ["Telefone"] = "17999999999", ["EmailInstitucional"] = "equipe@example.com"
        };
        if (entity == "Funcionario")
            await LoginFuncionarioTeste(client, app.Services);
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, $"/{entity}/Create", $"/{entity}/Create", fields)).StatusCode);
        if (entity == "Cliente")
            await LoginFuncionarioTeste(client, app.Services);
        var id = entity == "Cliente"
            ? app.Services.GetRequiredService<IClienteRepository>().Read().Single(c => c.Nome == "Pessoa").Id
            : app.Services.GetRequiredService<IFuncionarioRepository>().Read().Single(f => f.Nome == "Pessoa").Id;
        fields["Nome"] = "Nome editado";
        fields["Senha"] = "";
        fields["Id"] = "999";
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, $"/{entity}/Edit/{id}", $"/{entity}/Edit/{id}", fields)).StatusCode);
        Pessoa stored = entity == "Cliente" ? app.Services.GetRequiredService<IClienteRepository>().Read(id)!
            : app.Services.GetRequiredService<IFuncionarioRepository>().Read(id)!;
        Assert.Equal("Nome editado", stored.Nome);
        Assert.Equal("senha-original", stored.Senha);
        Assert.Equal(id, stored.Id);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/{entity}/Details/{id}")).StatusCode);
        fields.Remove("Id");
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, $"/{entity}/Delete/{id}", $"/{entity}/Delete/{id}", fields)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/{entity}/Details/{id}")).StatusCode);
    }

    [Theory]
    [InlineData("Ambiente")]
    [InlineData("Mesa")]
    public async Task CrudEstruturalEValidacao(string entity)
    {
        using var app = new AplicacaoTeste();
        using var client = app.Cliente();
        await LoginFuncionarioTeste(client, app.Services);
        Dictionary<string,string> fields = new()
        { ["Nome"] = "Sala", ["Numero"] = "5", ["Capacidade"] = "10", ["QtdLugares"] = "4", ["AmbienteId"] = "1", ["Status"] = "Livre" };
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, $"/{entity}/Create", $"/{entity}/Create", fields)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/{entity}/Index")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/{entity}/Details/1")).StatusCode);
        fields["Capacidade"] = "0"; fields["QtdLugares"] = "0";
        Assert.Equal(HttpStatusCode.OK, (await Post(client, $"/{entity}/Edit/1", $"/{entity}/Edit/1", fields)).StatusCode);
        fields["Capacidade"] = "12"; fields["QtdLugares"] = "6";
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, $"/{entity}/Edit/1", $"/{entity}/Edit/1", fields)).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, $"/{entity}/Delete/1", $"/{entity}/Delete/1", new() { ["Id"] = "1" })).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/{entity}/Details/1")).StatusCode);
    }

    [Fact]
    public async Task RestauranteTemRotaCorretaEStatusInvalidoNaoEhGravado()
    {
        using var app = new AplicacaoTeste();
        PrepararMesaTeste(app.Services);
        using var client = app.Cliente();
        await LoginFuncionarioTeste(client, app.Services);
        var menu = await client.GetStringAsync("/AreaRestaurante");
        Assert.Contains("/AreaRestaurante/Reservas", menu);
        await LoginClienteTeste(client, app.Services);
        await Post(client, "/Reserva/Create", "/Reserva/Create", DadosReserva());
        await LoginFuncionarioTeste(client, app.Services);
        var list = await client.GetStringAsync("/AreaRestaurante/Reservas");
        Assert.Contains("/AreaRestaurante/Edit/1", list);
        Assert.DoesNotContain("/AreaRestaurante/Delete/1", list);
        var response = await Post(client, "/AreaRestaurante/Edit/1", "/AreaRestaurante/Edit/1", new() { ["Status"] = "Desconhecido" });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Pendente", app.Services.GetRequiredService<IReservaRepository>().Read(1)!.Status);
        Assert.Contains("/AreaRestaurante/Reservas", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task MesasDisponiveisRespeitaDiaMesAno()
    {
        using var app = new AplicacaoTeste();
        var ambRepo = app.Services.GetRequiredService<IAmbienteRepository>();
        var mesaRepo = app.Services.GetRequiredService<IMesaRepository>();
        var resRepo = app.Services.GetRequiredService<IReservaRepository>();
        var cliRepo = app.Services.GetRequiredService<IClienteRepository>();

        ambRepo.Create(new Ambiente { Nome = "Sacada", Capacidade = 10, Status = "Disponivel" });
        mesaRepo.Create(new Mesa
        {
            Numero = 1,
            QtdLugares = 4,
            Status = "Livre",
            AmbienteId = 1,
            AmbienteNome = "Sacada"
        });
        cliRepo.Create(new Cliente
        {
            Nome = "Cliente mapa",
            Cpf = "11111111111",
            Telefone = "17999999999",
            EmailPessoal = "c@test.com",
            Senha = "x"
        });
        resRepo.Create(new Reserva
        {
            MesaId = 1,
            ClienteId = 1,
            DataHora = new DateTime(2027, 2, 2),
            QtdPessoas = 2
        });

        using var client = app.Cliente();
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/Auth/LoginCliente", "/Auth/LoginCliente", new Dictionary<string, string>
        {
            ["cpf"] = "11111111111",
            ["senha"] = "x"
        })).StatusCode);
        var noDiaReserva = await client.GetStringAsync("/Reserva/MesasDisponiveis?dataHora=2027-02-02&ambienteId=1");
        Assert.Contains("Reservada", noDiaReserva);

        var mesmoDiaOutroAno = await client.GetStringAsync("/Reserva/MesasDisponiveis?dataHora=2028-02-02&ambienteId=1");
        Assert.Contains("Livre", mesmoDiaOutroAno);
        Assert.DoesNotContain("Reservada", mesmoDiaOutroAno);
    }

    [Fact]
    public void AplicacaoRealRegistraSqlPorRequisicao()
    {
        using var app = new WebApplicationFactory<Program>();
        using var a = app.Services.CreateScope();
        using var b = app.Services.CreateScope();
        Assert.IsType<ClienteSqlRepository>(a.ServiceProvider.GetRequiredService<IClienteRepository>());
        Assert.IsType<FuncionarioSqlRepository>(a.ServiceProvider.GetRequiredService<IFuncionarioRepository>());
        Assert.IsType<MesaSqlRepository>(a.ServiceProvider.GetRequiredService<IMesaRepository>());
        Assert.IsType<AmbienteSqlRepository>(a.ServiceProvider.GetRequiredService<IAmbienteRepository>());
        Assert.IsType<ReservaSqlRepository>(a.ServiceProvider.GetRequiredService<IReservaRepository>());
        Assert.NotSame(a.ServiceProvider.GetRequiredService<IReservaRepository>(), b.ServiceProvider.GetRequiredService<IReservaRepository>());
    }
}
