using Microsoft.AspNetCore.Mvc;

namespace MaximusTitleLLC.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
