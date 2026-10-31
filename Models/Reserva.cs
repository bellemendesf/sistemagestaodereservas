// Importa os atributos de validação ([Required], [Range]...)
using System.ComponentModel.DataAnnotations;

// Define o namespace dos modelos
namespace GestaoReservas.Models;

// Modelo Reserva
public class Reserva
{
    // Propriedade Id
    public int Id { get; set; }
    // Propriedade Cliente
    public Cliente Cliente { get; set; } = new Cliente();
    // Propriedade ClienteId
    public int ClienteId { get; set; }
    // Validação: valor mínimo/máximo permitido
    [Range(1, int.MaxValue, ErrorMessage = "Informe uma mesa cadastrada.")]
    // Propriedade MesaId
    public int? MesaId { get; set; }
    // Propriedade DataHora
    public DateTime DataHora { get; set; }
    // Validação: valor mínimo/máximo permitido
    [Range(1, int.MaxValue, ErrorMessage = "Informe ao menos uma pessoa.")]
    // Propriedade QtdPessoas
    public int QtdPessoas { get; set; }
    // Propriedade Status
    public string Status { get; set; } = "";
}
