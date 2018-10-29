using Microsoft.EntityFrameworkCore;
using PasswordManager.Helper;

namespace PasswordManager.Model
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<FailedAccessAttemptCounter> FailedAccessAttemptCounters{ get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }
    }
}