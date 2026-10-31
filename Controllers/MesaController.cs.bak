
using GestaoReservas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

public class MesaController : Controller
{
    private readonly IMesaRepository repository;
    private readonly IAmbienteRepository ambienteRepository;

    public MesaController(
        IMesaRepository repository,
        IAmbienteRepository ambienteRepository)
    {
        this.repository = repository;
        this.ambienteRepository = ambienteRepository;
    }

    private void CarregarAmbientes(int? selecionado = null)
    {
        ViewBag.Ambientes = new SelectList(
            ambienteRepository.Read(),
            "Id",
            "Nome",
            selecionado
        );
    }

    public ActionResult Index()
    {
        return View("index", repository.Read());
    }

    [HttpGet]
    public ActionResult Create()
    {
        CarregarAmbientes();
        return View(new Mesa());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Mesa model)
    {
        if (!ModelState.IsValid)
        {
            CarregarAmbientes(model.AmbienteId);
            return View(model);
        }

        try
        {
            repository.Create(model);
            return RedirectToAction(nameof(Index));
        }
        catch (SqlException)
        {
            ModelState.AddModelError(
                "",
                "Não foi possível cadastrar a mesa. Verifique os dados e o ambiente selecionado."
            );

            CarregarAmbientes(model.AmbienteId);
            return View(model);
        }
    }

    [HttpGet]
    public ActionResult Edit(int id)
    {
        var model = repository.Read(id);

        if (model == null)
            return NotFound();

        CarregarAmbientes(model.AmbienteId);
        return View(model);
    }

    [HttpPost]
[ValidateAntiForgeryToken]
public ActionResult Edit(int id, Mesa model)
{
    // O ID original identifica a mesa no banco.
    // Ele não deve ser alterado pelo formulário.
    var mesaExistente = repository.Read(id);

    if (mesaExistente == null)
        return NotFound();

    model.Id = id;
    ModelState.Remove(nameof(model.Id));

    if (!ModelState.IsValid)
    {
        CarregarAmbientes(model.AmbienteId);
        return View(model);
    }

    try
    {
        repository.Update(model);
        return RedirectToAction(nameof(Index));
    }
    catch (SqlException)
    {
        ModelState.AddModelError(
            "",
            "Não foi possível salvar. Verifique os dados, o ambiente e os vínculos com reservas."
        );

        CarregarAmbientes(model.AmbienteId);
        return View(model);
    }
}
    [HttpGet]
    public ActionResult Details(int id)
    {
        var model = repository.Read(id);

        if (model == null)
            return NotFound();

        return View(model);
    }

    [HttpGet]
    public ActionResult Delete(int id)
    {
        var model = repository.Read(id);

        if (model == null)
            return NotFound();

        return View(model);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
        var model = repository.Read(id);

        if (model == null)
            return NotFound();

        try
        {
            repository.Delete(id);
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
{
    ModelState.AddModelError("", ex.Message);
    return View("Delete", model);
}
catch (SqlException)
{
    ModelState.AddModelError(
        "",
        "Não foi possível excluir a mesa. Verifique os vínculos existentes."
    );

    return View("Delete", model);
}
    }
}
