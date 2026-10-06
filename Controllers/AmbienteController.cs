using GestaoReservas.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoReservas.Controllers
{
    public interface IAmbienteController
    {
        ActionResult Create();
        ActionResult Create(Ambiente model);
        ActionResult Delete(int id);
        ActionResult Details(int id);  
        ActionResult Edit(int id);
        ActionResult Edit(Ambiente model);
        ActionResult Index();
    }

    public class AmbienteController : Controller, IAmbienteController
    {
        // "static" faz com que a lista seja armazenada na memória para todo o programa, não é necessário instanciar toda vez 
        private static List<Ambiente> ambientes = new List<Ambiente>
        {
            new Ambiente { Id = 1, Nome = "Externo", Capacidade = 3 },
            new Ambiente { Id = 2, Nome = "Interno", Capacidade = 4 },
            new Ambiente { Id = 3, Nome = "Sacada", Capacidade = 5 },
        };

        public ActionResult Index()
        {
            return View(ambientes);
        }

        public ActionResult Details(int id)
        {
            Ambiente ambiente = null;

            foreach (Ambiente a in ambientes)
            {
                if (a.Id == id)
                    ambiente = a;
            }

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
            model.Id = ambientes.Count + 1;
            ambientes.Add(model);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            Ambiente ambiente = null;

            foreach (Ambiente a in ambientes)
            {
                if (a.Id == id)
                    ambiente = a;
            }

            if (ambiente == null) return NotFound();
            return View(ambiente);
        }

        [HttpPost]
        public ActionResult Edit(Ambiente model)
        {
            foreach (Ambiente a in ambientes)
            {
                if (a.Id == model.Id)
                {
                    a.Nome = model.Nome;
                    a.Capacidade = model.Capacidade;
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            Ambiente ambiente = null;

            foreach (Ambiente a in ambientes)
            {
                if (a.Id == id)
                    ambiente = a;
            }

            if (ambiente == null) return NotFound();
            return View(ambiente);
        }
    }
}