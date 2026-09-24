using Microsoft.AspNetCore.Mvc;
namespace GestaoReservas.Models;

public class ReservasController : Controller
{
    private static List<Reserva> reservas = new List<Reserva>();

    public IActionResult Index()
    {
        return View(reservas);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Reserva reserva)
    {
        reserva.Id = reservas.Count + 1;

        reservas.Add(reserva);

        return RedirectToAction("Index");
    }
}