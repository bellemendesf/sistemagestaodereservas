using System.ComponentModel.DataAnnotations;

namespace GestaoReservas.Models;

public class Pessoa
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Informe o nome."), StringLength(50)]
    public string Nome { get; set; } = "";
    [Required(ErrorMessage = "Informe o CPF."), StringLength(20)]
    public string Cpf { get; set; } = "";
    [StringLength(100)]
    public string Senha { get; set; } = "";
}