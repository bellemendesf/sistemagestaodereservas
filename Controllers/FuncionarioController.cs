using GestaoReservas.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoReservas.Controllers
{
    public class FuncionarioController : Controller
    {
        // Em vez da lista static, guarda uma referência ao repositório
        private readonly IFuncionarioRepository _repositorio;

        // O ASP.NET Core injeta o repositório aqui automaticamente,
        // graças ao "AddScoped" que registramos no Program.cs
        public FuncionarioController(IFuncionarioRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public ActionResult Index()
        {
            var funcionarios = _repositorio.Read();   // busca TODOS no banco, em vez de usar a lista fixa
            return View(funcionarios);
        }

        public ActionResult Details(int id)
        {
            var funcionario = _repositorio.Read(id);   // busca UM no banco, usando o foreach que você tinha
            if (funcionario == null) return NotFound();
            return View(funcionario);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Funcionario model)
        {
            _repositorio.Create(model);   // manda o repositório inserir no banco
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var funcionario = _repositorio.Read(id);
            if (funcionario == null) return NotFound();
            return View(funcionario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Funcionario model)
        {
            _repositorio.Update(model);   // manda o repositório atualizar no banco
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var funcionario = _repositorio.Read(id);
            if (funcionario == null) return NotFound();
            return View(funcionario);
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