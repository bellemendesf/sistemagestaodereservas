using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using GestaoReservas.Models;

public class AreaRestauranteController : Controller
{
    private readonly IReservaRepository repository;

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
    public ActionResult Edit([FromRoute] int id)
    {
        var reserva = repository.Read(id);
        if (reserva == null) return NotFound();
        if (reserva.Status == "Cancelada") return BadRequest("Reserva cancelada não pode ser alterada.");

        return View("~/Views/Reserva/Edit.cshtml", reserva);
    }

    [HttpPost]
    public ActionResult Edit([FromRoute] int id, string status)
    {
        int reservaId = id;
        var reserva = repository.Read(reservaId);
        if (reserva == null) return NotFound();
        if (reserva.Status == "Cancelada") return BadRequest("Reserva cancelada não pode ser alterada.");

        // CORREÇÃO: apesar de a View enviar outros campos, funcionário só atualiza Status.
        if (status != "Pendente" &&
            status != "Confirmada" &&
            status != "Cancelada")
        {
            ModelState.AddModelError("Status", "Informe Pendente, Confirmada ou Cancelada.");
            return View("~/Views/Reserva/Edit.cshtml", reserva);
        }

        try
        {
            repository.UpdateStatus(reservaId, status);
        }
        catch (SqlException)
        {
            ModelState.AddModelError("", "Não foi possível salvar o status. Verifique a conexão com o banco.");
            return View("~/Views/Reserva/Edit.cshtml", reserva);
        }
        return RedirectToAction("Reservas");
    }

    [HttpGet]
    public ActionResult Details(int id)
    {
        var reserva = repository.Read(id);
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
