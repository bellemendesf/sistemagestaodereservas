// Importa os atributos de validação ([Required], [Range]...)
using System.ComponentModel.DataAnnotations;

// Define o namespace dos modelos
namespace GestaoReservas.Models;

// Modelo Cliente, que herda os campos de Pessoa
public class Cliente : Pessoa
{
    // Validação: campo obrigatório
    [Required(ErrorMessage = "Informe o telefone."), StringLength(20)]
    // Propriedade Telefone
    public string Telefone { get; set; } = "";
    // Propriedade EmailPessoal
    public string EmailPessoal { get; set; } = "";
   
}