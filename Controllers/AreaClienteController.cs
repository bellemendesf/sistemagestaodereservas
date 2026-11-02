using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[Authorize(AuthenticationSchemes = "ClienteScheme")]
public class AreaClienteController : Controller
{
    public IActionResult Index() => RedirectToAction("Index", "Reserva");
}
