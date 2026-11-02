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
    // Validação: tamanho máximo do texto
    // CORREÇÃO: mesmo tamanho da coluna senha varchar(30) do banco.
    [StringLength(30)]
    // Propriedade Senha
    public string Senha { get; set; } = "";
}