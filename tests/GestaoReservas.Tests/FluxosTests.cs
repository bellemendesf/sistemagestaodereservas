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
            services.AddSingleton<IAmbienteRepository, AmbienteMemoryRepository>();
            services.AddSingleton<IMesaRepository, MesaMemoryRepository>();
            services.AddSingleton<IClienteRepository, ClienteMemoryRepository>();
            services.AddSingleton<IFuncionarioRepository, FuncionarioMemoryRepository>();
            services.AddSingleton<IReservaRepository, ReservaMemoryRepository>();
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
        ["MesaId"] = "1", ["FuncionarioID"] = "1", ["DataHora"] = "2027-01-02T19:30",
        ["QtdPessoas"] = "2", ["Status"] = "Confirmada"
    };

    [Theory]
    [InlineData("/")]
    [InlineData("/AreaCliente")]
    [InlineData("/AreaRestaurante")]
    [InlineData("/Cliente/Create")]
    [InlineData("/Funcionario/Create")]
    [InlineData("/Ambiente/Create")]
    [InlineData("/Mesa/Create")]
    [InlineData("/Reserva/Create")]
    [InlineData("/AreaRestaurante/Reservas")]
    public async Task PaginasExistentesAbrem(string route)
    {
        using var app = new AplicacaoTeste();
        using var client = app.Cliente();
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
        foreach (var action in new[] { "Edit", "Details", "Delete" })
            Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/{entity}/{action}/999")).StatusCode);
    }

    [Fact]
    public async Task EscritaSemTokenEhRejeitada()
    {
        using var app = new AplicacaoTeste();
        using var client = app.Cliente();
        var response = await client.PostAsync("/Reserva/Create", new FormUrlEncodedContent(DadosReserva()));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Empty(app.Services.GetRequiredService<IReservaRepository>().Read());
    }

    [Fact]
    public async Task ReservaPreservaIdentidadeStatusEHistorico()
    {
        using var app = new AplicacaoTeste();
        using var client = app.Cliente();
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
        Assert.Equal(2, atual.MesaId);
        Assert.Equal(3, atual.QtdPessoas);
        Assert.Equal("Pendente", atual.Status);
        Assert.Equal(1, atual.FuncionarioID);
        Assert.Equal("Cliente de teste", atual.Cliente.Nome);

        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, $"/AreaRestaurante/Edit/{id}", $"/AreaRestaurante/Edit/{id}", new()
        { ["Status"] = "Confirmada", ["MesaId"] = "999", ["QtdPessoas"] = "999" })).StatusCode);
        Assert.Equal("Confirmada", repo.Read(id)!.Status);
        Assert.Equal(2, repo.Read(id)!.MesaId);
        Assert.Equal(3, repo.Read(id)!.QtdPessoas);

        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, $"/Reserva/Delete/{id}", $"/Reserva/Delete/{id}", new() { ["Id"] = id.ToString() })).StatusCode);
        Assert.Equal("Cancelada", Assert.Single(repo.Read()).Status);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync($"/Reserva/Edit/{id}")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync($"/AreaRestaurante/Edit/{id}")).StatusCode);
        // POST também deve impedir reativação, mesmo sem passar pelo formulário bloqueado.
        Assert.Equal(HttpStatusCode.BadRequest, (await Post(client, "/Reserva/Create", $"/AreaRestaurante/Edit/{id}", new() { ["Status"] = "Pendente" })).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await Post(client, "/Reserva/Create", $"/Reserva/Edit/{id}", edit)).StatusCode);
        Assert.Equal("Cancelada", repo.Read(id)!.Status);
    }

    [Fact]
    public async Task ReservaInvalidaNaoEhGravadaEReexibeDados()
    {
        using var app = new AplicacaoTeste();
        using var client = app.Cliente();
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
        using var client = app.Cliente();
        await Post(client, "/Reserva/Create", "/Reserva/Create", DadosReserva());
        var fields = DadosReserva();
        fields["QtdPessoas"] = "0";
        fields["DataHora"] = "nao-e-data";
        var response = await Post(client, "/Reserva/Edit/1", "/Reserva/Edit/1", fields);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var stored = app.Services.GetRequiredService<IReservaRepository>().Read(1)!;
        Assert.Equal(2, stored.QtdPessoas);
        Assert.Equal(new DateTime(2027, 1, 2, 19, 30, 0), stored.DataHora);
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
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, $"/{entity}/Create", $"/{entity}/Create", fields)).StatusCode);
        fields["Nome"] = "Nome editado";
        fields["Senha"] = "";
        fields["Id"] = "999";
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, $"/{entity}/Edit/1", $"/{entity}/Edit/1", fields)).StatusCode);
        Pessoa stored = entity == "Cliente" ? app.Services.GetRequiredService<IClienteRepository>().Read(1)!
            : app.Services.GetRequiredService<IFuncionarioRepository>().Read(1)!;
        Assert.Equal("Nome editado", stored.Nome);
        Assert.Equal("senha-original", stored.Senha);
        Assert.Equal(1, stored.Id);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/{entity}/Details/1")).StatusCode);
        fields.Remove("Id");
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, $"/{entity}/Delete/1", $"/{entity}/Delete/1", fields)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/{entity}/Details/1")).StatusCode);
    }

    [Theory]
    [InlineData("Ambiente")]
    [InlineData("Mesa")]
    public async Task CrudEstruturalEValidacao(string entity)
    {
        using var app = new AplicacaoTeste();
        using var client = app.Cliente();
        Dictionary<string,string> fields = new()
        { ["Nome"] = "Sala", ["Numero"] = "5", ["Capacidade"] = "10", ["QtdLugares"] = "4", ["Status"] = "Livre" };
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
        using var client = app.Cliente();
        var menu = await client.GetStringAsync("/AreaRestaurante");
        Assert.Contains("/AreaRestaurante/Reservas", menu);
        await Post(client, "/Reserva/Create", "/Reserva/Create", DadosReserva());
        var list = await client.GetStringAsync("/AreaRestaurante/Reservas");
        Assert.Contains("/AreaRestaurante/Edit/1", list);
        Assert.DoesNotContain("/AreaRestaurante/Delete/1", list);
        var response = await Post(client, "/AreaRestaurante/Edit/1", "/AreaRestaurante/Edit/1", new() { ["Status"] = "Desconhecido" });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Pendente", app.Services.GetRequiredService<IReservaRepository>().Read(1)!.Status);
        Assert.Contains("/AreaRestaurante/Reservas", await response.Content.ReadAsStringAsync());
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
