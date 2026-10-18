using Microsoft.AspNetCore.Mvc;
using GestaoReservas.Models;

public class ReservaController : Controller
{
    private IReservaRepository repository;

    public ReservaController(IReservaRepository repository)
    {
        this.repository = repository;
    }

    public ActionResult Index()
    {
        return View(repository.Read());
    }

    [HttpGet]
    public ActionResult Create()
    {
        Reserva model = new Reserva();
        model.Cliente = new Cliente();
        model.Cliente.Cpf = "";
        model.Cliente.EmailPessoal = "";
        model.Cliente.Senha = "";
        model.QtdPessoas = 2;
        model.Status = "Pendente";

        return View(model);
    }

    [HttpPost]
    public ActionResult Create(Reserva model)
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
    public ActionResult Edit(int id, Reserva model)
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
    public ActionResult Delete(Reserva model)
    {
        repository.Delete(model.Id);
        return RedirectToAction("Index");
    }
}
