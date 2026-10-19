using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    [Microsoft.AspNetCore.Mvc.IgnoreAntiforgeryToken]
    public ActionResult Error()
    {
        Response.StatusCode = 500;
        return View();
    }

    public ActionResult Index()
    {
        return View();
    }
}
