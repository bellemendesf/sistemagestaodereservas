// Variável builder
var builder = WebApplication.CreateBuilder(args);

// Habilita MVC com views
builder.Services.AddControllersWithViews(options =>
{
    // Exige token antifalsificação em todos os POST
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
}).AddMvcOptions(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

builder.Services.AddAuthentication()
    .AddCookie("FuncionarioScheme", options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.Cookie.Name = "FuncionarioAuth";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
    })
    .AddCookie("ClienteScheme", options =>
    {
        options.LoginPath = "/Auth/LoginCliente";
        options.AccessDeniedPath = "/Auth/AccessDeniedCliente";
        options.Cookie.Name = "ClienteAuth";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
    });
builder.Services.AddAuthorization();

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
builder.Services.AddScoped<IAuthRepository, AuthSqlRepository>();

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

app.UseAuthentication();
app.UseAuthorization();

// Rota padrão controller/action/id
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Inicia o servidor
app.Run();
// Permite inicialização da aplicação pelos testes de integração.
public partial class Program { }
