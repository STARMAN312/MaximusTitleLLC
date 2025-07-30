using MaximusTitleLLC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Services()
    {
        return View();
    }

    public IActionResult IndustryLinks()
    {
        return View();
    }

    public IActionResult CalculatorNTitleRates()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    [Route("/NoAccess")]
    public ActionResult NoAccess()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
