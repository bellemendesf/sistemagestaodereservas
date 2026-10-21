using System.ComponentModel.DataAnnotations;

namespace GestaoReservas.Models;

public class Mesa
{
    public int Id { get; set; }

    [Range(1, int.MaxValue,
        ErrorMessage = "Informe um número de mesa maior que zero.")]
    public int Numero { get; set; }

    [Range(1, int.MaxValue,
        ErrorMessage = "Informe ao menos um lugar.")]
    public int QtdLugares { get; set; }

    [Required(ErrorMessage = "Informe o status.")]
    [StringLength(20)]
    public string Status { get; set; } = "";
    [Range(1, int.MaxValue, ErrorMessage = "Selecione um ambiente.")]
    public int AmbienteId { get; set; }

    public string AmbienteNome { get; set; } = "";
}