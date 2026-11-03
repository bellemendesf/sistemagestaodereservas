// Importa os atributos de validação ([Required], [Range]...)
using System.ComponentModel.DataAnnotations;

namespace GestaoReservas.Models;

// Modelo Cliente, que herda os campos de Pessoa
public class Cliente : Pessoa
{
    // Validação: campo obrigatório
    [Required(ErrorMessage = "Informe o telefone."), StringLength(20)]
    public string Telefone { get; set; } = "";
    public string EmailPessoal { get; set; } = "";
   
}