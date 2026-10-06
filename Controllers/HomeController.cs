using Microsoft.AspNetCore.Mvc;
//port: http://localhost:5004/
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
//asp-controller + asp-action: eles dizem para qual Controller e qual método (Action) o link deve levar