using System.ComponentModel.DataAnnotations;

// Especialização de Pessoa no DER: herda os dados comuns e acrescenta telefone.
public class Cliente : Pessoa
{
    [Required(ErrorMessage = "Informe o telefone.")]
    public string Telefone { get; set; } = "";
}
