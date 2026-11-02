// Importa as classes do ASP.NET MVC (Controller, ActionResult...)
using GestaoReservas.Models;
using Microsoft.AspNetCore.Mvc;

// Controller responsável pelas telas de AreaCliente
public class AreaClienteController : Controller
{
    // Ação Index: mostra a listagem
    public ActionResult Index()
    {
        // Mostra a view com o modelo
        return View();
    }

    // CORREÇÃO: a área do cliente organiza os fluxos sem duplicar o CRUD de Reserva.
    public ActionResult Cadastrar()
    {
        // Redireciona para outra tela
        return RedirectToAction("Create", "Cliente");
    }

    // Ação NovaReserva
    public ActionResult NovaReserva()
    {
        // Redireciona para outra tela
        return RedirectToAction("Create", "Reserva");
    }

    // Ação ConsultarReservas
    public ActionResult ConsultarReservas()
    {
        // Redireciona para outra tela
        return RedirectToAction("Index", "Reserva");
    }

    // Ação EditarReserva
    public ActionResult EditarReserva(int id)
    {
        // Redireciona para outra tela
        return RedirectToAction("Edit", "Reserva", new { id });
    }

    // Ação CancelarReserva
    public ActionResult CancelarReserva(int id)
    {
        // CORREÇÃO: leva à confirmação; a mudança de status ocorrerá por POST.
        return RedirectToAction("Delete", "Reserva", new { id });
    }
     [HttpPost]
    public ActionResult Login(Cliente model)
    {
        Cliente usuarioCliente = repository.Login(model);

        if (usuarioCliente == null)
        {
            ViewBag.Error = "Usuário ou senha inválidos";
            return View(model);
        }

        HttpContext.Session.SetInt32("Id", usuarioCliente.Id);
        HttpContext.Session.SetString("Nome", usuarioCliente.Nome);

        return RedirectToAction("Index", "Cliente");
    }

    public ActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
