using Microsoft.AspNetCore.Mvc;

public class AreaClienteController : Controller
{
    public ActionResult Index()
    {
        return View();
    }

    // CORREÇÃO: a área do cliente organiza os fluxos sem duplicar o CRUD de Reserva.
    public ActionResult Cadastrar()
    {
        return RedirectToAction("Create", "Cliente");
    }

    public ActionResult NovaReserva()
    {
        return RedirectToAction("Create", "Reserva");
    }

    public ActionResult ConsultarReservas()
    {
        return RedirectToAction("Index", "Reserva");
    }

    public ActionResult EditarReserva(int id)
    {
        return RedirectToAction("Edit", "Reserva", new { id });
    }

    public ActionResult CancelarReserva(int id)
    {
        // CORREÇÃO: leva à confirmação; a mudança de status ocorrerá por POST.
        return RedirectToAction("Delete", "Reserva", new { id });
    }
}
