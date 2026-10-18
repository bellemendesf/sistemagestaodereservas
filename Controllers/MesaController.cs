using GestaoReservas.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoReservas.Controllers
{
    public class MesaController : Controller
    {
        // Em vez da lista static, guarda uma referência ao repositório
        private readonly IMesaRepository _repositorio;

        // O ASP.NET Core injeta o repositório aqui automaticamente,
        // graças ao "AddScoped" que registramos no Program.cs
        public MesaController(IMesaRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public ActionResult Index()
        {
            var mesas = _repositorio.Read();   // busca TODOS no banco, em vez de usar a lista fixa
            return View(mesas);
        }

        public ActionResult Details(int id)
        {
            var mesa = _repositorio.Read(id);   // busca UM no banco, usando o foreach que você tinha
            if (mesa == null) return NotFound();
            return View(mesa);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Mesa model)
        {
            _repositorio.Create(model);   // manda o repositório inserir no banco
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var mesa = _repositorio.Read(id);
            if (mesa == null) return NotFound();
            return View(mesa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Mesa model)
        {
            _repositorio.Update(model);   // manda o repositório atualizar no banco
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var mesa = _repositorio.Read(id);
            if (mesa == null) return NotFound();
            return View(mesa);
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