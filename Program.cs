var builder = WebApplication.CreateBuilder(args);

// CORREÇÃO: registra as implementações em memória das interfaces utilizadas pelos Controllers,
// seguindo o AddSingleton usado no projeto SistemaTarefas-main do professor.
builder.Services.AddSingleton<IAmbienteRepository>(new AmbienteMemoryRepository());
builder.Services.AddSingleton<IClienteRepository>(new ClienteMemoryRepository());
builder.Services.AddSingleton<IFuncionarioRepository>(new FuncionarioMemoryRepository());
builder.Services.AddSingleton<IMesaRepository>(new MesaMemoryRepository());
builder.Services.AddSingleton<IReservaRepository>(new ReservaMemoryRepository());

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
