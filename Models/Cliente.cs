namespace GestaoReservas.Models;

public class Cliente : Pessoa
{
    // CORREÇÃO: inicializa a propriedade para evitar aviso de nulidade no .NET.
    public string Telefone { get; set; } = "";
}
