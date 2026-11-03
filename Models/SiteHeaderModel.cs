namespace GestaoReservas.Models;

public class SiteHeaderModel
{
    public bool FuncionarioLogado { get; init; }
    public bool ClienteLogado { get; init; }
    public string? Nome { get; init; }
    public string? Perfil { get; init; }
}
