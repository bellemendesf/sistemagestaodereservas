using GestaoReservas.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoReservas.Controllers
{
    public class AmbienteController : Controller
    {
        // Em vez da lista static, guarda uma referência ao repositório
        private readonly IAmbienteRepository _repositorio;

        // O ASP.NET Core injeta o repositório aqui automaticamente,
        // graças ao "AddScoped" que registramos no Program.cs
        public AmbienteController(IAmbienteRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public ActionResult Index()
        {
            var ambientes = _repositorio.Read();   // busca TODOS no banco, em vez de usar a lista fixa
            return View(ambientes);
        }

        public ActionResult Details(int id)
        {
            var ambiente = _repositorio.Read(id);   // busca UM no banco, usando o foreach que você tinha
            if (ambiente == null) return NotFound();
            return View(ambiente);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Ambiente model)
        {
            _repositorio.Create(model);   // manda o repositório inserir no banco
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var ambiente = _repositorio.Read(id);
            if (ambiente == null) return NotFound();
            return View(ambiente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ambiente model)
        {
            _repositorio.Update(model);   // manda o repositório atualizar no banco
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var ambiente = _repositorio.Read(id);
            if (ambiente == null) return NotFound();
            return View(ambiente);
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