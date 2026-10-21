using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using GestaoReservas.Models;

public class ReservaController : Controller
{
    private readonly IReservaRepository repository;

    public ReservaController(IReservaRepository repository)
    {
        this.repository = repository;
    }

    public ActionResult Index() => View(repository.Read());

    [HttpGet]
    public ActionResult Create() => View(new Reserva { QtdPessoas = 2, Status = "Pendente" });

    [HttpPost]
    public ActionResult Create(Reserva model)
    {
        model.Status = "Pendente";
        ModelState.Remove(nameof(model.Status));
        if (model.Cliente == null)
            ModelState.AddModelError(nameof(model.Cliente), "Informe os dados do cliente.");
        if (model.DataHora == default)
            ModelState.AddModelError(nameof(model.DataHora), "Informe a data e hora.");
        if (!ModelState.IsValid) return View(model);
        try
        {
            repository.Create(model);
            return RedirectToAction(nameof(Index));
        }
        catch (SqlException)
        {
            ModelState.AddModelError("", "Não foi possível cadastrar. Verifique CPF, telefone, e mesa cadastrados e a conexão com o banco.");
            return View(model);
        }
    }

    [HttpGet]
    public ActionResult Edit([FromRoute] int id)
    {
        var reserva = repository.Read(id);
        if (reserva == null) return NotFound();
        if (reserva.Status == "Cancelada") return BadRequest("Reserva cancelada não pode ser editada.");
        return View(reserva);
    }

    [HttpPost]
    public ActionResult Edit([FromRoute] int id, Reserva model)
    {
        var original = repository.Read(id);
        if (original == null) return NotFound();
        if (original.Status == "Cancelada") return BadRequest("Reserva cancelada não pode ser editada.");

        model.Id = id;
        model.Cliente = original.Cliente;
        model.ClienteId = original.ClienteId;
        model.Status = original.Status;
        // Esses dados não fazem parte da edição pelo cliente.
        foreach (var key in ModelState.Keys.Where(k =>
            k == "Id" || k == "Cliente" || k.StartsWith("Cliente.") ||
            k == "ClienteId" || k == "Status").ToList())
            ModelState.Remove(key);
        if (model.DataHora == default)
            ModelState.AddModelError(nameof(model.DataHora), "Informe a data e hora.");
        if (!ModelState.IsValid) return View(model);
        try
        {
            repository.Update(model);
            return RedirectToAction(nameof(Index));
        }
        catch (SqlException)
        {
            ModelState.AddModelError("", "Não foi possível salvar. Verifique a mesa informada e a conexão com o banco.");
            return View(model);
        }
    }

    [HttpGet]
    public ActionResult Details(int id)
    {
        var model = repository.Read(id);
        if (model == null) return NotFound();
        return View(model);
    }

    [HttpGet]
    public ActionResult Delete(int id)
    {
        var model = repository.Read(id);
        if (model == null) return NotFound();
        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed([FromRoute] int id)
    {
        var model = repository.Read(id);
        if (model == null) return NotFound();
        try
        {
            repository.Delete(id); // Cancelamento lógico: preserva o histórico.
            return RedirectToAction(nameof(Index));
        }
        catch (SqlException)
        {
            ModelState.AddModelError("", "Não foi possível cancelar. Verifique a conexão com o banco.");
            return View("Delete", model);
        }
    }
}
