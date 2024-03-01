using Microsoft.AspNetCore.Mvc;
using Movies.Business.globals;
using Movies.Business.users;
using Movies.Interface;
using System.Net;

namespace Streamit_movie_mvc.Controllers.Main
{
    public class RegisterController : Controller
    {
        private readonly IUserService _userService;

        public RegisterController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View("Register");
        }

        public async Task<IActionResult> Register(RegisterUser registerUser)
        {
            ResponseDTO response = await _userService.Register(registerUser);

            if(response.Status == HttpStatusCode.Created)
            {
                return RedirectToAction("VerifyCode", "Auth", (Guid)response.Data);
            }

            ViewBag.response = response;
            return View(registerUser);
        }
    }
}
