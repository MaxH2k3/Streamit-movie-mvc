using Microsoft.AspNetCore.Mvc;

namespace Streamit_movie_mvc.Controllers.Dashboard
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
