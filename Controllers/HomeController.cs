// Importa as classes do ASP.NET MVC (Controller, ActionResult...)
using Microsoft.AspNetCore.Mvc;

// Controller responsável pelas telas de Home
public class HomeController : Controller
{
    // Dispensa o token antifalsificação nesta ação
    [Microsoft.AspNetCore.Mvc.IgnoreAntiforgeryToken]
    // Ação Error
    public ActionResult Error()
    {
        // Define o código HTTP da resposta
        Response.StatusCode = 500;
        // Mostra a view com o modelo
        return View();
    }

    // Ação Index: mostra a listagem
    public ActionResult Index()
    {
        // Mostra a view com o modelo
        return View();
    }
}
