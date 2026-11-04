using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using GestaoReservas.Models;

public class AuthController : Controller
{
    private const string EsquemaFuncionario = "FuncionarioScheme";
    private const string EsquemaCliente = "ClienteScheme";
    private readonly IAuthRepository auth;

    public AuthController(IAuthRepository auth) => this.auth = auth;

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login() => View("LoginFuncionario", new LoginViewModel());

    [AllowAnonymous]
    [HttpGet]
    public IActionResult LoginCliente() => View(new LoginViewModel());

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> LoginCliente(LoginViewModel model)
    {
        var cliente = auth.LoginCliente(model);
        if (cliente == null)
        {
            ViewBag.Error = "CPF ou senha inválidos.";
            return View(model);
        }

        await HttpContext.SignOutAsync(EsquemaFuncionario);

        await HttpContext.SignInAsync(
            EsquemaCliente,
            new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, cliente.Id.ToString()),
                new Claim(ClaimTypes.Name, cliente.Nome),
                new Claim(ClaimTypes.Role, "Cliente")
            ], EsquemaCliente)),
            new AuthenticationProperties { ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(5) });

        return RedirectToAction("Index", "Reserva");
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var funcionario = auth.Login(model);
        if (funcionario == null)
        {
            ViewBag.Error = "CPF ou senha inválidos.";
            return View("LoginFuncionario", model);
        }

        await HttpContext.SignOutAsync(EsquemaCliente);

        await HttpContext.SignInAsync(
            EsquemaFuncionario,
            new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, funcionario.Id.ToString()),
                new Claim(ClaimTypes.Name, funcionario.Nome),
                new Claim(ClaimTypes.Role, "Funcionario")
            ], EsquemaFuncionario)),
            new AuthenticationProperties { ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(5) });

        return RedirectToAction("Index", "AreaRestaurante");
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(EsquemaFuncionario);
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> LogoutCliente()
    {
        await HttpContext.SignOutAsync(EsquemaCliente);
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> SairConta()
    {
        await HttpContext.SignOutAsync(EsquemaFuncionario);
        await HttpContext.SignOutAsync(EsquemaCliente);
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    public IActionResult AccessDenied() => View();

    [AllowAnonymous]
    public IActionResult AccessDeniedCliente() => View();
}
