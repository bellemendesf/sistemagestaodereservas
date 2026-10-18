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
        repository.Create(model);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public ActionResult Edit(int id)
    {
        return View(repository.Read(id));
    }

    [HttpPost]
    public ActionResult Edit(int id, Mesa model)
    {
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
    public ActionResult Delete(Mesa model)
    {
        repository.Delete(model.Id);
        return RedirectToAction("Index");
    }
}
