using System.ComponentModel.DataAnnotations;

// Especialização de Pessoa no DER: herda os dados comuns e acrescenta e-mail institucional.
public class Funcionario : Pessoa
{
    [Required(ErrorMessage = "Informe o e-mail institucional.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail institucional válido.")]
    public string EmailInstitucional { get; set; } = "";
}
