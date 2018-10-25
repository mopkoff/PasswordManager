using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PasswordManager.Helper;
using PasswordManager.Model;

namespace PasswordManager.Pages.my
{

    public class EditModel : PageModel
    {
        private readonly ApplicationContext _context; 

        [BindProperty]
        public Account Account { get; set; }
        private IConfiguration _configuration;
        private EncryptHelper encryptHelper;
        private User currentUser;
        public string HashAlgorithm;

        public EditModel(ApplicationContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
            HashAlgorithm =
                configuration.GetValue<string>("HashAlgorithm");
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Account = await _context.Accounts.FindAsync(id);
            encryptHelper = new EncryptHelper(await _context.Users.FindAsync(Account.UserId), HashAlgorithm);

            Account.Password = encryptHelper.Decrypt(Account.Password);

            if (Account == null)
            {
                return RedirectToPage("/Passwords");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            encryptHelper = new EncryptHelper(await _context.Users.FindAsync(Account.UserId), HashAlgorithm);
            Account.Password = encryptHelper.Encrypt(Account.Password);

            _context.Attach(Account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new Exception($"Account {Account.Id}  not found", e);
            }

            return RedirectToPage("./Passwords");

        }
    }

}