namespace GestaoReservas.Models;
public class Reserva
{
    public int Id {get;set;}
    public required Cliente Cliente {get;set;}
    public required int MesaId {get;set;}
    public required int FuncionarioID {get;set;}
    public required DateTime DataHora {get;set;}
    public required int QtdPessoas {get;set;}
    public required string Status {get;set;}
    
}