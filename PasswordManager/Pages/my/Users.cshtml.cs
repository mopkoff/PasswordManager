using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PasswordManager.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PasswordManager.Pages.my
{
    public class UsersModel : PageModel
    {
        private readonly ApplicationContext _context;
        public List<User> Users { get; set; }
        private string[] Roles;

        public UsersModel(ApplicationContext db, IConfiguration _configuration)
        {
            _context = db;
            Roles = _configuration.GetValue<string>("Roles").Split(_configuration.GetValue<string>("RolesSeparator"));
        }
        public void OnGet()
        {
            if (((ClaimsIdentity)User.Identity).Claims
                   .Where(c => c.Type == ClaimTypes.Role)
                   .Select(c => c.Value).ToList().Contains(Roles[1]))
            {
                Users = _context.Users.AsNoTracking().ToList();
            }
            else
                Users = _context.Users.AsNoTracking().Where(user => user.Login == HttpContext.User.Identity.Name).ToList();

        }
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);

            await HttpContext.SignOutAsync();
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Users");
        }
    }
}