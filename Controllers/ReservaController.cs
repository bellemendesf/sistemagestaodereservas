using Microsoft.AspNetCore.Mvc;
using GestaoReservas.Models;
namespace GestaoReservas.Controllers;

public class ReservaController : Controller
{
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
    var reserva = reservas.FirstOrDefault(r => r.Id == id); // Busca a reserva com o ID fornecido
    //   firstOrDefault retorna o primeiro elemento que satisfaz a condição ou null se não encontrar nenhum. 

    if (reserva == null) // Se não encontrar a reserva, retorna NotFound
    {
        return NotFound();
    }

    return View(reserva);
}
    public IActionResult Edit(int id) //GET
    {
        var reserva = reservas.FirstOrDefault(r => r.Id == id); // Busca a reserva com o ID fornecido
        if (reserva == null)
        {
            return NotFound();
        }
        return View(reserva);
    }
    [HttpPost]
    public IActionResult Edit(Reserva reserva) //POST
    {
        var existingReserva = reservas.FirstOrDefault(r => r.Id == reserva.Id); // Busca a reserva existente com o mesmo ID
        if (existingReserva == null)
        {
            return NotFound();
        }

        // Atualiza os campos da reserva existente com os valores do formulário
        existingReserva.MesaId = reserva.MesaId;
        existingReserva.FuncionarioID = reserva.FuncionarioID;
        existingReserva.DataHora = reserva.DataHora;
        existingReserva.QtdPessoas = reserva.QtdPessoas;
        existingReserva.Status = reserva.Status;

        return RedirectToAction("Index");
    }
    public IActionResult Delete(int id) //GET
    {
        var reserva = reservas.FirstOrDefault(r => r.Id == id); // Busca a reserva com o ID fornecido
        if (reserva == null)
        {
            return NotFound();
        }
        return View(reserva);
    }
    [HttpPost, ActionName("Delete")]
public IActionResult DeleteConfirmed(int id) // POST
{
    var reserva = reservas.FirstOrDefault(r => r.Id == id);

    if (reserva == null)
    {
        return NotFound();
    }

    reservas.Remove(reserva);

    return RedirectToAction("Index");
}
}