using GestaoReservas.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoReservas.Controllers
{
    public class ClienteController : Controller
    {
        // Em vez da lista static, guarda uma referência ao repositório
        private readonly IClienteRepository _repositorio;

        // O ASP.NET Core injeta o repositório aqui automaticamente,
        // graças ao "AddScoped" que registramos no Program.cs
        public ClienteController(IClienteRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public ActionResult Index()
        {
            var clientes = _repositorio.Read();   // busca TODOS no banco, em vez de usar a lista fixa
            return View(clientes);
        }

        public ActionResult Details(int id)
        {
            var clientes = _repositorio.Read(id);   // busca UM no banco, usando o foreach que você tinha
            if (clientes == null) return NotFound();
            return View(clientes);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Cliente model)
        {
            _repositorio.Create(model);   // manda o repositório inserir no banco
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var cliente = _repositorio.Read(id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Cliente model)
        {
            _repositorio.Update(model);   // manda o repositório atualizar no banco
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var cliente = _repositorio.Read(id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _repositorio.Delete(id);   // manda o repositório apagar do banco
            return RedirectToAction("Index");
        }
    }
}