using Microsoft.AspNetCore.Mvc;
using Streamit_movie_mvc.Models;
using System.Diagnostics;

namespace Streamit_movie_mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Genres()
        {
            return View("Body/Genres");
        }
        
        public IActionResult Cast()
        {
            return View("Body/Cast");
        }
        
        public IActionResult Movie()
        {
            return View("Body/Movie");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
