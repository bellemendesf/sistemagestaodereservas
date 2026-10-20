using System.ComponentModel.DataAnnotations;

namespace GestaoReservas.Models;

public class Cliente : Pessoa
{
    [Required(ErrorMessage = "Informe o telefone."), StringLength(20)]
    public string Telefone { get; set; } = "";
    public string EmailPessoal { get; set; } = "";
   
}