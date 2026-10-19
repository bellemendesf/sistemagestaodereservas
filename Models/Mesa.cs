namespace GestaoReservas.Models;

public class Mesa
{
    public int Id { get; set; }
    public int Numero { get; set; }
    public int QtdLugares { get; set; }
    // CORREÇÃO: inicializa a propriedade para evitar aviso de nulidade no .NET.
    public string Status { get; set; } = "";

    public int AmbienteId { get; set; }
}
