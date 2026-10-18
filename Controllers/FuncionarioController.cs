using Microsoft.AspNetCore.Mvc;
using GestaoReservas.Models;

public class FuncionarioController : Controller
{
    private IFuncionarioRepository repository;

    public FuncionarioController(IFuncionarioRepository repository)
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
        Funcionario model = new Funcionario();
        return View(model);
    }

    [HttpPost]
    public ActionResult Create(Funcionario model)
    {
        repository.Create(model);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public ActionResult Edit(int id)
    {
        return View(repository.Read(id));
    }

    [HttpPost]
    public ActionResult Edit(int id, Funcionario model)
    {
        // CORREÇÃO: a View Edit não envia campo Id; recupera o identificador da rota.
        model.Id = id;
        repository.Update(model);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public ActionResult Details(int id)
    {
        return View(repository.Read(id));
    }

    [HttpGet]
    public ActionResult Delete(int id)
    {
        return View(repository.Read(id));
    }

    [HttpPost]
    // CORREÇÃO: o formulário Delete envia o ID pela URL, não como propriedade do model.
    public ActionResult Delete(int id)
    {
        repository.Delete(id);
        return RedirectToAction("Index");
    }
}
