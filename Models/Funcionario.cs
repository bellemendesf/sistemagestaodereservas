using System.ComponentModel.DataAnnotations;

namespace GestaoReservas.Models;

public class Funcionario : Pessoa
{
    public int Codigo { get; set; }
    [Required(ErrorMessage = "Informe o e-mail institucional."), StringLength(50)]
    public string EmailInstitucional { get; set; } = "";
}
