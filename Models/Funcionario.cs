namespace GestaoReservas.Models;

public class Funcionario : Pessoa
{
    public int Codigo { get; set; }
    // CORREÇÃO: inicializa a propriedade para evitar aviso de nulidade no .NET.
    public string EmailInstitucional { get; set; } = "";
}
