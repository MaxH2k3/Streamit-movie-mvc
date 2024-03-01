using Microsoft.AspNetCore.Mvc;
using Movies.Interface;
using Streamit_movie_mvc.Utilities;
using System.Net;

namespace Streamit_movie_mvc.Controllers.Verify
{
    public class VerifyController : Controller
    {
        private readonly IUserService _userService;

        public VerifyController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> VerifyCode(int code, Guid userId)
        {
            var response = await _userService.VerifyAccount(code.ToString(), userId, (int)Constraint.TypeEmail.CODE);
            if(response.Status == HttpStatusCode.OK)
            {
                return RedirectToAction("Index", "Login");
            }

            return RedirectToAction("VerifyCode", "Auth");
        }

    }
}
