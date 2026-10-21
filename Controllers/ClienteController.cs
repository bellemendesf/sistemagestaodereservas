using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using GestaoReservas.Models;

public class ClienteController : Controller
{
    private readonly IClienteRepository repository;

    public ClienteController(IClienteRepository repository)
    {
        this.repository = repository;
    }

    public ActionResult Index() => View("index", repository.Read());

    [HttpGet]
    public ActionResult Create() => View(new Cliente());

    [HttpPost]
    public ActionResult Create(Cliente model)
    {
        if (string.IsNullOrWhiteSpace(model.EmailPessoal))
            ModelState.AddModelError(nameof(model.EmailPessoal), "Informe o e-mail pessoal.");
        if (string.IsNullOrWhiteSpace(model.Senha))
            ModelState.AddModelError(nameof(model.Senha), "Informe a senha.");
        if (!ModelState.IsValid) return View(model);
        try
        {
            repository.Create(model);
            return RedirectToAction(nameof(Index));
        }
        catch (SqlException)
        {
            ModelState.AddModelError("", "Não foi possível cadastrar. Verifique os dados, duplicidades e a conexão com o banco.");
            return View(model);
        }
    }

    [HttpGet]
    public ActionResult Edit([FromRoute] int id)
    {
        var model = repository.Read(id);
        if (model == null) return NotFound();
        return View(model);
    }

    [HttpPost]
    public ActionResult Edit([FromRoute] int id, Cliente model)
    {
        // A rota identifica o registro; não permita trocar a identidade pelo formulário.
        model.Id = id;
        ModelState.Remove(nameof(model.Id));
        if (repository.Read(id) == null) return NotFound();
        if (string.IsNullOrWhiteSpace(model.EmailPessoal))
            ModelState.AddModelError(nameof(model.EmailPessoal), "Informe o e-mail pessoal.");
        if (!ModelState.IsValid) return View(model);
        try
        {
            repository.Update(model);
            return RedirectToAction(nameof(Index));
        }
        catch (SqlException)
        {
            ModelState.AddModelError("", "Não foi possível salvar. Verifique os dados, duplicidades e vínculos existentes.");
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
            repository.Delete(id);
            return RedirectToAction(nameof(Index));
        }
        catch (SqlException)
        {
            ModelState.AddModelError("", "Não foi possível excluir. O cadastro pode estar vinculado a outros registros; verifique também a conexão.");
            return View("Delete", model);
        }
    }
}
