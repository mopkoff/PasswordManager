using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PasswordManager.Model;
using Microsoft.EntityFrameworkCore;

namespace PasswordManager.Pages.my
{
    public class UsersModel : PageModel
    {
        private readonly ApplicationContext _context;
        public List<User> Users { get; set; }
        public UsersModel(ApplicationContext db)
        {
            _context = db;
        }
        public void OnGet()
        {
            Users = _context.Users.AsNoTracking().ToList();
        }
    }
}