// Importa os atributos de validação ([Required], [Range]...)
using System.ComponentModel.DataAnnotations;

// Define o namespace dos modelos
namespace GestaoReservas.Models;

// Modelo Mesa
public class Mesa
{
    // Propriedade Id
    public int Id { get; set; }

    // Validação: valor mínimo/máximo permitido
    [Range(1, int.MaxValue,
        // Atribui ErrorMessage
        ErrorMessage = "Informe um número de mesa maior que zero.")]
    // Propriedade Numero
    public int Numero { get; set; }

    // Validação: valor mínimo/máximo permitido
    [Range(1, int.MaxValue,
        // Atribui ErrorMessage
        ErrorMessage = "Informe ao menos um lugar.")]
    // Propriedade QtdLugares
    public int QtdLugares { get; set; }

    // Validação: campo obrigatório
    [Required(ErrorMessage = "Informe o status.")]
    // Validação: tamanho máximo do texto
    [StringLength(20)]
    // Propriedade Status
    public string Status { get; set; } = "";
    // Validação: valor mínimo/máximo permitido
    [Range(1, int.MaxValue, ErrorMessage = "Selecione um ambiente.")]
    // Propriedade AmbienteId
    public int AmbienteId { get; set; }

    // Propriedade AmbienteNome
    public string AmbienteNome { get; set; } = "";
}