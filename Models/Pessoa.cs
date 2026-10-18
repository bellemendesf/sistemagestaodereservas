namespace GestaoReservas.Models;

public class Pessoa
{
    public int Id { get; set; }
    // CORREÇÃO: inicializa os campos de texto para evitar avisos de nulidade no .NET.
    public string Nome { get; set; } = "";
    public string Cpf { get; set; } = "";
    public string EmailPessoal { get; set; } = "";
    public string Senha { get; set; } = "";
}
