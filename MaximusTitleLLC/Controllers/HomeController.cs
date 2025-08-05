using MaximusTitleLLC.Data;
using MaximusTitleLLC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendMessage(ContactFormVM model, string returnUrl)
    {
        var hCaptchaResponse = Request.Form["h-captcha-response"];
        var secret = "ES_d11b0fc28be94272a55c053004cd84eb";

        if (string.IsNullOrEmpty(hCaptchaResponse))
        {
            TempData["NotificationMessage"] = "Captcha verification failed.";
            return RedirectToAction("Index");
        }

        using var http = new HttpClient();
        var values = new Dictionary<string, string>
        {
            { "response", hCaptchaResponse },
            { "secret", secret }
        };

        var content = new FormUrlEncodedContent(values);
        var response = await http.PostAsync("https://hcaptcha.com/siteverify", content);
        var json = await response.Content.ReadAsStringAsync();

        var result = System.Text.Json.JsonSerializer.Deserialize<HCaptchaVerifyResponse>(json);

        if (result == null || !result.success)
        {
            TempData["NotificationMessage"] = "Captcha verification failed.";
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index");
        }

        if (ModelState.IsValid)
        {

            try
            {
                ContactForm contact = new ContactForm
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    Message = model.Message,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.ContactForms.AddAsync(contact);
                await _context.SaveChangesAsync();

                TempData["NotificationMessage"] = "Message sent successfully!";
            }
            catch (DbUpdateException dbEx)
            {
                TempData["NotificationMessage"] = "There was a problem saving your message. Please try again later.";
                Console.WriteLine(dbEx);
            }
            catch (Exception ex)
            {
                TempData["NotificationMessage"] = "An unexpected error occurred. Please try again later.";
                Console.WriteLine(ex);
            }

        }
        else
        {
            TempData["NotificationMessage"] = "Please fill all the fields before submitting your contact form.";
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index");
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
