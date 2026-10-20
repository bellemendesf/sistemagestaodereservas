using System.ComponentModel.DataAnnotations;

namespace GestaoReservas.Models;

public class Reserva
{
    public int Id { get; set; }
    public Cliente Cliente { get; set; } = new Cliente();
    public int ClienteId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Informe uma mesa cadastrada.")]
    public int MesaId { get; set; }
    public DateTime DataHora { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Informe ao menos uma pessoa.")]
    public int QtdPessoas { get; set; }
    public string Status { get; set; } = "";
}
