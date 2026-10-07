namespace GestaoReservas.Models;

public class Funcionario: Pessoa
{
    public int Codigo {get;set;}
    public required string Email {get;set;}
    public required string Senha {get;set;}
}