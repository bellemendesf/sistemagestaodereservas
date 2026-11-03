using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private const string EsquemaFuncionario = "FuncionarioScheme";
    private const string EsquemaCliente = "ClienteScheme";

    [Microsoft.AspNetCore.Mvc.IgnoreAntiforgeryToken]
    public ActionResult Error()
    {
        Response.StatusCode = 500;
        return View();
    }

    public async Task<IActionResult> Index()
    {
        var funcionario = await HttpContext.AuthenticateAsync(EsquemaFuncionario);
        var cliente = await HttpContext.AuthenticateAsync(EsquemaCliente);

        var funcionarioLogado = funcionario.Succeeded;
        var clienteLogado = cliente.Succeeded && !funcionarioLogado;

        ViewBag.FuncionarioLogado = funcionarioLogado;
        ViewBag.ClienteLogado = clienteLogado;
        ViewBag.NomeFuncionario = funcionarioLogado ? funcionario.Principal?.Identity?.Name : null;
        ViewBag.NomeCliente = clienteLogado ? cliente.Principal?.Identity?.Name : null;

        return View();
    }
}
