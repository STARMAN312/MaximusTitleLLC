using MaximusTitleLLC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MaximusTitleLLC.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ContactForm> ContactForms { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
    }
}
