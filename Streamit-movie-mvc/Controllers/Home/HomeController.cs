using Microsoft.AspNetCore.Mvc;
using Movies.Business.globals;
using Movies.Interface;
using Newtonsoft.Json;
using Streamit_movie_mvc.Models.Domain;
using System.Diagnostics;
using Ubiety.Dns.Core;

namespace Streamit_movie_mvc.Controllers.Home;

public class HomeController : Controller
{
    private readonly IMovieService _movieService;
    private readonly ICategoryService _categoryService;

    public HomeController(IMovieService movieService, ICategoryService categoryService)
    {
        _movieService = movieService;
        _categoryService = categoryService;
    }

    public IActionResult Index()
    {
        IEnumerable<Movie> movies = _movieService.GetMovies();
        var tempResponse = TempData["response"] as string;
        ResponseDTO responseDTO;
        if(!string.IsNullOrEmpty(tempResponse))
        {
            responseDTO = JsonConvert.DeserializeObject<ResponseDTO>(tempResponse);
            ViewBag.response = responseDTO;
            
        }

        return View(movies);
    }

    public IActionResult Genres()
    {
        var categories = _categoryService.GetCategories();
        return View("Body/Genres", categories);
    }

    public IActionResult Cast()
    {
        return View("Body/Cast");
    }

    public IActionResult Movie()
    {
        return View("Body/Movie");
    }

    public IActionResult MovieDetail()
    {
        return View("Body/MovieDetail");
    }

    public IActionResult AccountDetail()
    {
        return View("Body/AccountDetail");
    }

    public IActionResult PricingPlan()
    {
        return View("Body/PricingPlan");
    }

    public IActionResult PersonDetail()
    {
        return View("Body/PersonDetail");
    }

    public IActionResult PlayList()
    {
        return View("Body/PlayList");
    }
}
