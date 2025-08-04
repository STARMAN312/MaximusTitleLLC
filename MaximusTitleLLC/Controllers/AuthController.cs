using Mailjet.Client.Resources;
using MaximusTitleLLC.Data;
using MaximusTitleLLC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MaximusTitleLLC.Controllers
{
    public class AuthController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LogInVM Model)
        {
            if (!ModelState.IsValid)
                return View(Model);

            var appUser = await _userManager.FindByNameAsync(Model.Username);

            if (appUser == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(Model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                appUser.UserName!,
                Model.Password,
                isPersistent: true,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {

                var session = new UserSession
                {
                    UserId = appUser.Id.ToString(),
                    ExpiresAt = DateTime.UtcNow.AddHours(12),
                    IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = HttpContext.Request.Headers["User-Agent"].ToString(),
                };

                _context.UserSessions.Add(session);
                await _context.SaveChangesAsync();

                Response.Cookies.Append("auth_session_id", session.SessionId.ToString(), new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = session.ExpiresAt
                });

                var user = await _userManager.FindByNameAsync(Model.Username);

                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles.Contains("Admin"))
                        return Redirect("https://guardiancapitolllc.com/");

                    if (roles.Contains("Client"))
                        return Redirect("https://guardiancapitolllc.com/Account");
                }
            }

            ModelState.AddModelError(string.Empty, "An error ocurred while logging you in, pleas try again later.");
            return View(Model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM Model)
        {
            if (!ModelState.IsValid)
                return View(Model);

            var existingUser = await _userManager.FindByEmailAsync(Model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                return View(Model);
            }

            var existingUsername = await _userManager.FindByNameAsync(Model.Username);
            if (existingUsername != null)
            {
                ModelState.AddModelError("Username", "This username is already taken. Please choose another.");
                return View(Model);
            }

            ApplicationUser newUser = new()
            {
                UserName = Model.Username,
                FullName = Model.FullName,
                Email = Model.Email,
                PersonalEmail = Model.Email,
            };

            IdentityResult createResult = await _userManager.CreateAsync(newUser, Model.Password);

            if (createResult.Succeeded)
            {
                List<BankAccount> accounts = new List<BankAccount>
                    {
                        new BankAccount
                        {
                            Type = BankAccount.AccountType.Checking,
                            Balance = 0,
                            UserId = newUser.Id,
                            AccountNumber = GenerateUniqueAccountNumber(),
                        },
                        new BankAccount
                        {
                            Type = BankAccount.AccountType.Savings,
                            Balance = 0,
                            UserId = newUser.Id,
                            AccountNumber = GenerateUniqueAccountNumber(),
                        },
                        new BankAccount
                        {
                            Type = BankAccount.AccountType.TrustFund,
                            Balance = 0,
                            UserId = newUser.Id,
                            AccountNumber = GenerateUniqueAccountNumber(),
                        }
                    };

                _context.BankAccounts.AddRange(accounts);
                await _context.SaveChangesAsync();

                await _userManager.AddToRoleAsync(newUser, "Client");
                await _signInManager.SignInAsync(newUser, isPersistent: true);

                var session = new UserSession
                {
                    UserId = newUser.Id,
                    ExpiresAt = DateTime.UtcNow.AddHours(12),
                    IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = HttpContext.Request.Headers["User-Agent"].ToString(),
                };

                _context.UserSessions.Add(session);
                await _context.SaveChangesAsync();

                Response.Cookies.Append("auth_session_id", session.SessionId.ToString(), new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = session.ExpiresAt
                });

                return Redirect("https://guardiancapitolllc.com/Account");
            }

            foreach (var error in createResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(Model);
        }

        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            var sessionId = Request.Cookies["auth_session_id"];
            if (Guid.TryParse(sessionId, out var id))
            {
                var session = await _context.UserSessions.FirstOrDefaultAsync(s => s.SessionId == id.ToString());
                if (session != null)
                {
                    session.IsActive = false;
                    await _context.SaveChangesAsync();
                }
            }

            Response.Cookies.Delete("auth_session_id");

            return RedirectToAction("Index", "Auth");
        }

        private string GenerateUniqueAccountNumber()
        {
            string number;
            do
            {
                number = Generate12DigitAccountNumber();
            }
            while (_context.BankAccounts.Any(a => a.AccountNumber == number));

            return number;
        }
        private string Generate12DigitAccountNumber()
        {
            byte[] buffer = new byte[8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(buffer);
            }

            ulong value = BitConverter.ToUInt64(buffer, 0);
            ulong number = value % 1_000_000_000_000;

            return number.ToString("D12");
        }

    }
}
