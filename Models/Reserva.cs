namespace GestaoReservas.Models;
public class Reserva
{
    public int Id {get;set;}
    public  Cliente? Cliente {get;set;}
    public  int MesaId {get;set;}
    public  int FuncionarioID {get;set;}
    public  DateTime DataHora {get;set;}
    public  int QtdPessoas {get;set;}
    public  string? Status {get;set;}
    
}