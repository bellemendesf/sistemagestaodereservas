namespace GestaoReservas.Models;

public class Reserva
{
    public int Id { get; set; }
    // CORREÇÃO: a View de criação acessa Cliente.Nome, Cliente.Cpf e outros campos.
    public int ClienteId { get; set; }
    public int MesaId { get; set; }
    public int FuncionarioID { get; set; }
    public DateTime DataHora { get; set; }
    public int QtdPessoas { get; set; }
    // CORREÇÃO: inicializa a propriedade para evitar aviso de nulidade no .NET.
    public string Status { get; set; } = "";
}
