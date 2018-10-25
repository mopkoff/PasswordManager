using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PasswordManager.Model;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Identity;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Helper;
using Microsoft.Extensions.Configuration;

namespace PasswordManager.Pages.my
{
    public class PasswordsModel : PageModel
    {
        private readonly ApplicationContext _context;
        public List<Account> Accounts{ get; set; }
        public string ContextUser { get; set; }
        private User CurrentUser;
        private EncryptHelper encryptHelper;
        private IConfiguration _configuration;
        public string HashAlgorithm;

        public PasswordsModel(ApplicationContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
            HashAlgorithm =
                configuration.GetValue<string>("HashAlgorithm");
        }
        public void OnGet()
        {
            ContextUser = HttpContext.User.Identity.Name;
            CurrentUser = _context.Users.Where(user => user.Login == ContextUser).ToList().Last();

            Accounts = _context.Accounts
                .AsNoTracking()
                .Where(account => account.UserId == CurrentUser.Id).ToList()
                .ToList();

            if (Accounts.Count != 0)
            {
                encryptHelper = new EncryptHelper(CurrentUser, HashAlgorithm);
                Accounts.ForEach(account => account.Password = encryptHelper.Decrypt(account.Password));
            }
        }
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var account = await _context.Accounts.FindAsync(id);

            if (account != null)
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Passwords");
        }
    }
}