var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Registra as implementações SQL das interfaces, usadas pelos Controllers.
// AddScoped: uma nova instância (e conexão) a cada requisição — ideal para acesso a banco.
builder.Services.AddScoped<IAmbienteRepository, AmbienteSqlRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteSqlRepository>();
builder.Services.AddScoped<IFuncionarioRepository, FuncionarioSqlRepository>();
builder.Services.AddScoped<IMesaRepository, MesaSqlRepository>();
builder.Services.AddScoped<IReservaRepository, ReservaSqlRepository>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();