using GestaoReservas.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoReservas.Controllers
{
    public class MesaController : Controller
    {
        // "static" faz a lista durar entre as páginas (sem isso, ela reseta toda hora)
        private static List<Mesa> mesas = new List<Mesa>
        {
            new Mesa { Id = 1, Numero = 1, QtdLugares = 4, Status = "Livre" },
            new Mesa { Id = 2, Numero = 2, QtdLugares = 4, Status = "Reservada" },
            new Mesa { Id = 3, Numero = 3, QtdLugares = 4, Status = "Ocupada" },
        };

        private static int proximoId = 4;   // controla o próximo Id a ser usado

        public ActionResult Index()
        {
            return View(mesas);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Mesa model)
        {
            model.Id = proximoId;
            proximoId++;   // prepara o próximo número pra mesa seguinte

            mesas.Add(model);

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            Mesa mesa = BuscarMesaPorId(id);
            if (mesa == null) return NotFound();
            return View(mesa);
        }

        [HttpPost]
        public ActionResult Edit(Mesa model)
        {
            Mesa mesa = BuscarMesaPorId(model.Id);
            if (mesa == null) return NotFound();

            mesa.Numero = model.Numero;
            mesa.QtdLugares = model.QtdLugares;
            mesa.Status = model.Status;

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            Mesa mesa = BuscarMesaPorId(id);
            if (mesa == null) return NotFound();
            return View(mesa);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Mesa mesa = BuscarMesaPorId(id);
            if (mesa == null) return NotFound();

            mesas.Remove(mesa);
            return RedirectToAction("Index");
        }

        // Método auxiliar: procura uma mesa pelo Id dentro da lista, "no braço"
        private Mesa BuscarMesaPorId(int id)
        {
            foreach (Mesa mesa in mesas)
            {
                if (mesa.Id == id)
                    return mesa;
            }
            return null;   // não encontrou nenhuma com esse id
        }
    }
}
             
