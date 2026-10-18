namespace GestaoReservas.Models;

public class Ambiente
{
    public int Id { get; set; }
    // CORREÇÃO: inicializa campos obrigatórios para evitar avisos de nulidade no .NET.
    public string Nome { get; set; } = "";
    public string Status { get; set; } = "";
    public int Capacidade { get; set; }
}
