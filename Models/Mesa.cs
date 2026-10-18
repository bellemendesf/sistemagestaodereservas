namespace GestaoReservas.Models;

public class Mesa
{
    public int Id { get; set; }
    public int Numero { get; set; }
    public int QtdLugares { get; set; }
    public string Status { get; set; } = "";
}
