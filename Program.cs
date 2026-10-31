// Variável builder
var builder = WebApplication.CreateBuilder(args);

// Habilita MVC com views
builder.Services.AddControllersWithViews(options =>
{
    // Exige token antifalsificação em todos os POST
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
}).AddMvcOptions(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

// UNIÃO: os Controllers da versão A utilizam os repositórios SQL da versão B.
// AddScoped: uma nova instância (e conexão) a cada requisição — ideal para acesso a banco.
builder.Services.AddScoped<IAmbienteRepository, AmbienteSqlRepository>();
// Registra ClienteSqlRepository como implementação de IClienteRepository
builder.Services.AddScoped<IClienteRepository, ClienteSqlRepository>();
// Registra FuncionarioSqlRepository como implementação de IFuncionarioRepository
builder.Services.AddScoped<IFuncionarioRepository, FuncionarioSqlRepository>();
// Registra MesaSqlRepository como implementação de IMesaRepository
builder.Services.AddScoped<IMesaRepository, MesaSqlRepository>();
// Registra ReservaSqlRepository como implementação de IReservaRepository
builder.Services.AddScoped<IReservaRepository, ReservaSqlRepository>();

// Variável app
var app = builder.Build();

// O perfil HTTP local funciona sem certificado de desenvolvimento.
if (!app.Environment.IsDevelopment())
{
    // Página de erro em produção
    app.UseExceptionHandler("/Home/Error");
    // Ativa HSTS em produção
    app.UseHsts();
    // Redireciona para HTTPS
    app.UseHttpsRedirection();
}

// Libera os arquivos de wwwroot (css, imagens)
app.UseStaticFiles();

// Ativa o roteamento
app.UseRouting();

// Rota padrão controller/action/id
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Inicia o servidor
app.Run();
// Permite inicialização da aplicação pelos testes de integração.
public partial class Program { }
