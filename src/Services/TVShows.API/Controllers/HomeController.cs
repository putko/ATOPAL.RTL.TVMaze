namespace AUTOPAL.RTL.TVMaze.Services.TVShows.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return new RedirectResult(url: "~/swagger");
        }
    }
}