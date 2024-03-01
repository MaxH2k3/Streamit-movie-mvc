using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Streamit_movie_mvc.Controllers.Main
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HandleException(int statusCode)
        {
            if(statusCode == 404)
                return View("NotFound");

            return View("ServerError");
        }
    }
}
