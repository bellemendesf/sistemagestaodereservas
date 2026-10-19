var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
}).AddMvcOptions(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

// UNIÃO: os Controllers da versão A utilizam os repositórios SQL da versão B.
// AddScoped: uma nova instância (e conexão) a cada requisição — ideal para acesso a banco.
builder.Services.AddScoped<IAmbienteRepository, AmbienteSqlRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteSqlRepository>();
builder.Services.AddScoped<IFuncionarioRepository, FuncionarioSqlRepository>();
builder.Services.AddScoped<IMesaRepository, MesaSqlRepository>();
builder.Services.AddScoped<IReservaRepository, ReservaSqlRepository>();

var app = builder.Build();

// O perfil HTTP local funciona sem certificado de desenvolvimento.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
// Permite inicialização da aplicação pelos testes de integração.
public partial class Program { }
