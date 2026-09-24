namespace GestaoReservas.Models;

public  class Ambiente
{
    public int Id {get;set;}
    public required string Nome {get;set;}
    public required string Status {get;set;}
    public required int Capacidade {get;set;}
}