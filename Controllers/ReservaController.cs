using Microsoft.AspNetCore.Mvc;
using GestaoReservas.Models;
public class ReservasController : Controller
{
   public IActionResult Index()
   {
      return View();
   }
   public IActionResult Create()
   {
      return View();
   }
[HttpPost]
   public IActionResult Create(Reserva reserva)
   {
      if (ModelState.IsValid)
      {
         // Lógica para salvar a reserva no banco de dados
         return RedirectToAction("Index");
      }
      return View(reserva);
   }




}
