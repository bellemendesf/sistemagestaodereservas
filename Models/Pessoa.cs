namespace GestaoReservas.Models;

public class Pessoa
{
    public int Id{get; set;}
    public required string Nome {get;set;}
    public required string Cpf {get;set;}
    public required string EmailPessoal {get;set;}
    public required string Senha {get;set;}
}