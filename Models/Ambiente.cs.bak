using System.ComponentModel.DataAnnotations;

namespace GestaoReservas.Models;

public class Ambiente
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Informe o nome.")]
    public string Nome { get; set; } = "";
    [StringLength(20)]
    public string Status { get; set; } = "";
    [Range(1, int.MaxValue, ErrorMessage = "A capacidade deve ser maior que zero.")]
    public int Capacidade { get; set; }
}
