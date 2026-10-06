using Microsoft.AspNetCore.Mvc;
using GestaoReservas.Models;

namespace GestaoReservas.Controllers
{
    public class ReservaController : Controller
    {
        // "static" faz a lista durar entre as páginas (sem isso, ela reseta toda hora)
        private static List<Reserva> reservas = new List<Reserva>();

        public IActionResult Index()
        {
            return View(reservas);
        }

        public IActionResult Create()
        {
            return View(new Reserva());
        }

        [HttpPost]
        public IActionResult Create(Reserva reserva)
        {
            reserva.Id = reservas.Count + 1;
            reservas.Add(reserva);
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            Reserva reserva = null;

            foreach (Reserva r in reservas)
            {
                if (r.Id == id)
                    reserva = r;
            }

            if (reserva == null) return NotFound();
            return View(reserva);
        }

        public IActionResult Edit(int id) // GET
        {
            Reserva reserva = null;

            foreach (Reserva r in reservas)
            {
                if (r.Id == id)
                    reserva = r;
            }

            if (reserva == null) return NotFound();
            return View(reserva);
        }

        [HttpPost]
        public IActionResult Edit(Reserva reserva) // POST
        {
            foreach (Reserva r in reservas)
            {
                if (r.Id == reserva.Id)
                {
                    r.Cliente = reserva.Cliente;       // estava faltando atualizar este campo
                    r.MesaId = reserva.MesaId;
                    r.FuncionarioID = reserva.FuncionarioID;
                    r.DataHora = reserva.DataHora;
                    r.QtdPessoas = reserva.QtdPessoas;
                    r.Status = reserva.Status;
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id) // GET
        {
            Reserva reserva = null;

            foreach (Reserva r in reservas)
            {
                if (r.Id == id)
                    reserva = r;
            }

            if (reserva == null) return NotFound();
            return View(reserva);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id) // POST
        {
            Reserva reserva = null;

            foreach (Reserva r in reservas)
            {
                if (r.Id == id)
                    reserva = r;
            }

            if (reserva != null)
                reservas.Remove(reserva);

            return RedirectToAction("Index");
        }
    }
}