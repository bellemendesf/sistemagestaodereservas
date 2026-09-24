namespace GestaoReservas.Models;

public class Mesa
{
    public int Id {get;set;}
    public required int Numero {get;set;}
    public required int QtdLugares {get;set;}
    public required string Status {get;set;}
}