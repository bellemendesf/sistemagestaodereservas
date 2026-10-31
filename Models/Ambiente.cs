// Importa os atributos de validação ([Required], [Range]...)
using System.ComponentModel.DataAnnotations;

// Define o namespace dos modelos
namespace GestaoReservas.Models;

// Modelo Ambiente
public class Ambiente
{
    // Propriedade Id
    public int Id { get; set; }
    // Validação: campo obrigatório
    [Required(ErrorMessage = "Informe o nome.")]
    // Propriedade Nome
    public string Nome { get; set; } = "";
    // Validação: tamanho máximo do texto
    [StringLength(20)]
    // Propriedade Status
    public string Status { get; set; } = "";
    // Validação: valor mínimo/máximo permitido
    [Range(1, int.MaxValue, ErrorMessage = "A capacidade deve ser maior que zero.")]
    // Propriedade Capacidade
    public int Capacidade { get; set; }
}
