
using Microsoft.AspNetCore.Mvc;

namespace GestaoReservas.Controllers
{
    public class AreaClienteController : Controller
    {
        // Abre a página inicial da área do cliente
        public IActionResult Index()
        {
            return View();
        }
    }
}
