// Importa os atributos de validação ([Required], [Range]...)
using System.ComponentModel.DataAnnotations;

// Define o namespace dos modelos
namespace GestaoReservas.Models;

// Modelo Funcionario, que herda os campos de Pessoa
public class Funcionario : Pessoa
{
    // Validação: campo obrigatório
    [Required(ErrorMessage = "Informe o e-mail institucional.")]
    // Propriedade EmailInstitucional
    public string EmailInstitucional { get; set; } = "";
}
