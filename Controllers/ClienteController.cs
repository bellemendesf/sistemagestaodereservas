using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using GestaoReservas.Models;

public class ClienteController : Controller
{
    private IClienteRepository repository;

    public ClienteController(IClienteRepository repository)
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
        Cliente model = new Cliente();
        return View(model);
    }

    [HttpPost]
    public ActionResult Create(Cliente model)
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
        Cliente model = repository.Read(id);
        if (model == null) return NotFound();
        return View(model);
    }

    [HttpPost]
    public ActionResult Edit(int id, Cliente model)
    {
        // CORREÇÃO: a View Edit não envia campo Id; recupera o identificador da rota.
        model.Id = id;
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
        Cliente model = repository.Read(id);
        if (model == null) return NotFound();
        return View(model);
    }

    [HttpGet]
    public ActionResult Delete(int id)
    {
        Cliente model = repository.Read(id);
        if (model == null) return NotFound();
        return View(model);
    }

    // UNIÃO: nome de ação de B evita a assinatura duplicada em A.
    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
        try
        {
            repository.Delete(id);
        }
        catch (SqlException)
        {
            return BadRequest("Não foi possível gravar. Verifique os dados obrigatórios, registros duplicados, vínculos existentes e a conexão com o banco.");
        }
        return RedirectToAction("Index");
    }
}
