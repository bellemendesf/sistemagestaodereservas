using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using GestaoReservas.Models;

public class MesaController : Controller
{
    private IMesaRepository repository;

    public MesaController(IMesaRepository repository)
    {
        this.repository = repository;
    }

    public ActionResult Index()
    {
        // CORREÇÃO: as Views existentes usam "index.cshtml" em minúsculas;
        // informa o nome exato para funcionar também em sistemas com distinção de maiúsculas.
        return View("index", repository.Read());
    }

    [HttpGet]
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Create(Mesa model)
    {
        try
        {
            repository.Create(model);
        }
        catch (SqlException)
        {
            return BadRequest("Não foi possível gravar. Verifique os dados obrigatórios, registros duplicados, vínculos existentes e a conexão com o banco.");
        }
        return RedirectToAction("Index");
    }

    [HttpGet]
    public ActionResult Edit(int id)
    {
        Mesa model = repository.Read(id);
        if (model == null) return NotFound();
        return View(model);
    }

    [HttpPost]
    public ActionResult Edit(int id, Mesa model)
    {
        model.Id = id > 0 ? id : model.Id;
        if (repository.Read(model.Id) == null) return NotFound();
        try
        {
            repository.Update(model);
        }
        catch (SqlException)
        {
            return BadRequest("Não foi possível gravar. Verifique os dados obrigatórios, registros duplicados, vínculos existentes e a conexão com o banco.");
        }
        return RedirectToAction("Index");
    }

    [HttpGet]
    public ActionResult Details(int id)
    {
        Mesa model = repository.Read(id);
        if (model == null) return NotFound();
        return View(model);
    }

    [HttpGet]
    public ActionResult Delete(int id)
    {
        Mesa model = repository.Read(id);
        if (model == null) return NotFound();
        return View(model);
    }

    [HttpPost]
    public ActionResult Delete(Mesa model)
    {
        try
        {
            repository.Delete(model.Id);
        }
        catch (SqlException)
        {
            return BadRequest("Não foi possível gravar. Verifique os dados obrigatórios, registros duplicados, vínculos existentes e a conexão com o banco.");
        }
        return RedirectToAction("Index");
    }
}
