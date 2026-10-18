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

   [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
    repository.Delete(id);
    return RedirectToAction("Index");
    }
}
