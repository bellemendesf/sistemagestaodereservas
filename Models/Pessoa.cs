// Importa os atributos de validação ([Required], [Range]...)
using System.ComponentModel.DataAnnotations;

// Define o namespace dos modelos
namespace GestaoReservas.Models;

// Modelo Pessoa
public class Pessoa
{
    // Propriedade Id
    public int Id { get; set; }
    // Validação: campo obrigatório
    [Required(ErrorMessage = "Informe o nome."), StringLength(50)]
    // Propriedade Nome
    public string Nome { get; set; } = "";
    // Validação: campo obrigatório
    [Required(ErrorMessage = "Informe o CPF."), StringLength(20)]
    // Propriedade Cpf
    public string Cpf { get; set; } = "";
    // Validação: tamanho máximo compatível com hash de senha no banco (varchar 256)
    [StringLength(256)]
    // Propriedade Senha (texto puro só na tela; no banco fica o hash)
    public string Senha { get; set; } = "";
}
