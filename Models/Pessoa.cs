using System.ComponentModel.DataAnnotations;

// Classe base do DER: reúne os dados comuns de clientes e funcionários.
// Neste CRUD os controllers mantêm esses dados apenas na memória.
public class Pessoa
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome.")]
    public string Nome { get; set; } = "";

    // CPF é texto para preservar zeros à esquerda; não valida dígitos verificadores.
    [Required(ErrorMessage = "Informe o CPF.")]
    public string Cpf { get; set; } = "";

    [Required(ErrorMessage = "Informe o e-mail pessoal.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail pessoal válido.")]
    public string EmailPessoal { get; set; } = "";

    // Recebe a senha do formulário; o controller armazena somente seu hash.
    // Na edição, um valor vazio mantém a senha já cadastrada.
    public string? Senha { get; set; }
}
