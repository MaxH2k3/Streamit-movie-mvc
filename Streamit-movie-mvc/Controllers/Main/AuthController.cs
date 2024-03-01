using Microsoft.AspNetCore.Mvc;
using Movies.Business.users;

namespace Streamit_movie_mvc.Controllers.Main
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult VerifyCode(Guid id)
        {
            Console.WriteLine(id);
            return View();
        }

    }
}
