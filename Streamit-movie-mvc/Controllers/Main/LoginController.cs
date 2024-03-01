using Microsoft.AspNetCore.Mvc;
using Streamit_movie_mvc.Models.DTO;
using Movies.Interface;
using System.Net;
using Newtonsoft.Json;
using Movies.Security;
using Streamit_movie_mvc.Models.Domain;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace Streamit_movie_mvc.Controllers.Main
{
    
    public class LoginController : Controller
    {
        private readonly IUserService _userService;
        private readonly JWTGenerator _jwtGenerator;

        public LoginController(IUserService userService,
            JWTGenerator jWTGenerator)
        {
            _userService = userService;
            _jwtGenerator = jWTGenerator;
        }

        public IActionResult Index()
        {
            var token = Request.Cookies["AccessToken"];

            if (!string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Home");
            }

            return View("Login");
        }

        public async Task<IActionResult> LoginAsync(UserDTO userDTO)
        {
            if(userDTO.UserName != null && userDTO.Password != null)
            {
                var response = _userService.Login(userDTO);
                if (response?.Status == HttpStatusCode.OK)
                {
                    var user = response.Data as User;

                    CookieOptions cookieOptions = new CookieOptions
                    {
                        // Set the secure flag, which Chrome's changes will require for SameSite none.
                        // Note this will also require you to be running on HTTPS.
                        Secure = true,

                        // Set the cookie to HTTP only which is good practice unless you really do need
                        // to access it client side in scripts.
                        HttpOnly = true,

                        // Add the SameSite attribute, this will emit the attribute with a value of none.
                        SameSite = SameSiteMode.None,

                        // The client should follow its default cookie policy.
                        // SameSite = SameSiteMode.Unspecified

                        Expires = DateTime.Now.AddMonths(1)

                    };

                    //set information to cookie
                    Response.Cookies.Append("DisplayName", user.DisplayName, cookieOptions);
                    Response.Cookies.Append("Avatar", user.Avatar, cookieOptions);
                    Response.Cookies.Append("AccessToken", _jwtGenerator.GenerateToken(userDTO), cookieOptions);

                    return RedirectToAction("Index", "Home");
                }
                ViewBag.response = response;
            }
            
            return View();
        }

        [HttpPost]
        public string LoginWithGoogleAsync(LoginWithDTO loginWithDTO)
        {
            CookieOptions cookieOptions = new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None,

                Expires = DateTime.Now.AddMonths(1)

            };

            //set information to cookie
            Response.Cookies.Append("DisplayName", loginWithDTO.DisplayName, cookieOptions);
            Response.Cookies.Append("Avatar", loginWithDTO.Avatar, cookieOptions);
            Response.Cookies.Append("AccessToken", loginWithDTO.AccessToken, cookieOptions);

            return "Login Successfully!";
        }

        [HttpPost]
        public string LoginWithMicrosoft(LoginWithDTO loginWithDTO)
        {
            CookieOptions cookieOptions = new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None,

                Expires = DateTime.Now.AddMonths(1)

            };

            //set information to cookie
            Response.Cookies.Append("DisplayName", loginWithDTO.DisplayName, cookieOptions);
            Response.Cookies.Append("Avatar", loginWithDTO.Avatar, cookieOptions);
            Response.Cookies.Append("AccessToken", loginWithDTO.AccessToken, cookieOptions);

            return "Login Successfully!";
        }

        public IActionResult ForgotPassword()
        {
            return View("ForgotPassword");
        }

        public IActionResult Logout()
        {
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            TempData["Response"] = JsonConvert.SerializeObject(new { Status = HttpStatusCode.OK, Message = "Logout Successfully!" });

            return RedirectToAction("Index", "Home");
        }

    }
}
