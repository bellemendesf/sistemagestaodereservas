using Microsoft.AspNetCore.Mvc;

namespace GestaoReservas.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}