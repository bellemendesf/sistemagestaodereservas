using Microsoft.AspNetCore.Mvc;
using GestaoReservas.Models;

public class AreaRestauranteController : Controller
{
    private IReservaRepository repository;

    // CORREÇÃO: injeta a mesma interface já registrada no Program.cs.
    public AreaRestauranteController(IReservaRepository repository)
    {
        this.repository = repository;
    }

    public ActionResult Index()
    {
        return View();
    }

    // CORREÇÃO: organiza as ações do funcionário sem replicar o CRUD de mesas e ambientes.
    public ActionResult CadastrarFuncionario()
    {
        return RedirectToAction("Create", "Funcionario");
    }

    public ActionResult GerenciarMesas()
    {
        return RedirectToAction("Index", "Mesa");
    }

    public ActionResult GerenciarAmbientes()
    {
        return RedirectToAction("Index", "Ambiente");
    }

    public ActionResult Reservas()
    {
        // CORREÇÃO: reutiliza a View existente; links Edit levam ao fluxo do restaurante.
        return View("~/Views/Reserva/Index.cshtml", repository.Read());
    }

    [HttpGet]
    public ActionResult Edit(int id)
    {
        Reserva reserva = repository.Read(id);
        if (reserva == null) return NotFound();
        if (reserva.Status == "Cancelada") return BadRequest("Reserva cancelada não pode ser alterada.");
        // CORREÇÃO: reutiliza a View de edição existente sem modificá-la.
        return View("~/Views/Reserva/Edit.cshtml", reserva);
    }

    [HttpPost]
    public ActionResult Edit(int id, Reserva model)
    {
        int reservaId = id > 0 ? id : model.Id;
        Reserva reserva = repository.Read(reservaId);
        if (reserva == null) return NotFound();
        if (reserva.Status == "Cancelada") return BadRequest("Reserva cancelada não pode ser alterada.");

        // CORREÇÃO: apesar de a View enviar outros campos, funcionário só atualiza Status.
        if (model.Status != "Pendente" &&
            model.Status != "Confirmada" &&
            model.Status != "Cancelada")
        {
            ModelState.AddModelError("Status", "Informe Pendente, Confirmada ou Cancelada.");
            return View("~/Views/Reserva/Edit.cshtml", reserva);
        }

        repository.UpdateStatus(reservaId, model.Status);
        return RedirectToAction("Reservas");
    }

    [HttpGet]
    public ActionResult Details(int id)
    {
        Reserva reserva = repository.Read(id);
        if (reserva == null) return NotFound();
        return View("~/Views/Reserva/Details.cshtml", reserva);
    }

    [HttpGet]
    public ActionResult Delete(int id)
    {
        // CORREÇÃO: funcionário não possui ação de excluir reserva.
        return StatusCode(403);
    }

    [HttpPost]
    public ActionResult Delete(Reserva model)
    {
        // CORREÇÃO: impede exclusão no fluxo específico do restaurante.
        return StatusCode(403);
    }

    [HttpGet]
    public ActionResult Create()
    {
        // CORREÇÃO: criar reserva não é uma operação do funcionário.
        return StatusCode(403);
    }
}
