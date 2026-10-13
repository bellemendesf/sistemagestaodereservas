
using Microsoft.AspNetCore.Mvc;

namespace GestaoReservas.Controllers
{
    public class AreaRestauranteController : Controller
    {
        // Abre a página inicial da área do restaurante
        public IActionResult Index()
        {
            return View();
        }
    }
}
