using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

public class AuthController : Controller
{
    private const string Esquema = "FuncionarioScheme";
    private readonly IAuthRepository auth;

    public AuthController(IAuthRepository auth) => this.auth = auth;

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login() => View();

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(string cpf, string senha)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            ModelState.AddModelError("", "Informe o CPF.");
        if (string.IsNullOrWhiteSpace(senha))
            ModelState.AddModelError("", "Informe a senha.");
        if (!ModelState.IsValid)
            return View();

        var funcionario = auth.Login(cpf, senha);
        if (funcionario == null)
        {
            ModelState.AddModelError("", "CPF ou senha inválidos.");
            return View();
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, funcionario.Id.ToString()),
            new Claim(ClaimTypes.Name, funcionario.Nome),
            new Claim(ClaimTypes.Role, "Funcionario")
        };

        await HttpContext.SignInAsync(
            Esquema,
            new ClaimsPrincipal(new ClaimsIdentity(claims, Esquema)),
            new AuthenticationProperties { ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(5) });

        return RedirectToAction("Index", "AreaRestaurante");
    }

    [Authorize(AuthenticationSchemes = Esquema)]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(Esquema);
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    public IActionResult AccessDenied() => View();
}
