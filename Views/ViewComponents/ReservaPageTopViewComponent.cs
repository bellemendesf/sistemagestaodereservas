using GestaoReservas.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

public class ReservaPageTopViewComponent : ViewComponent
{
    private const string EsquemaFuncionario = "FuncionarioScheme";
    private const string EsquemaCliente = "ClienteScheme";

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var funcionario = await HttpContext.AuthenticateAsync(EsquemaFuncionario);
        var cliente = await HttpContext.AuthenticateAsync(EsquemaCliente);

        if (funcionario.Succeeded)
        {
            return View(new SiteHeaderModel
            {
                FuncionarioLogado = true,
                Nome = funcionario.Principal?.Identity?.Name,
                Perfil = "Funcionario"
            });
        }

        if (cliente.Succeeded)
        {
            return View(new SiteHeaderModel
            {
                ClienteLogado = true,
                Nome = cliente.Principal?.Identity?.Name,
                Perfil = "Cliente"
            });
        }

        return View(new SiteHeaderModel());
    }
}
