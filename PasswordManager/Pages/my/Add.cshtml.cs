using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PasswordManager.Model;
using PasswordManager.Helper;
using Microsoft.Extensions.Configuration;

using System.Security.Claims;
namespace PasswordManager.Pages.my
{
    public class AddModel : PageModel
    {
        private readonly ApplicationContext _context;
        [BindProperty]
        public Account Account { get; set; }
        public User CurrentUser { get; set; }
        private EncryptHelper encryptHelper;
        private IConfiguration configuration;
        public string HashAlgorithm;

        public AddModel(ApplicationContext context, IConfiguration _configuration)
        {
            _context = context;
            Account = new Account();
            configuration = _configuration;
            HashAlgorithm =
                configuration.GetValue<string>("HashAlgorithm");
        }
        public void OnGet()
        {
            CurrentUser = _context.Users.Where(user => user.Login == HttpContext.User.Identity.Name).ToList().Last();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                CurrentUser = _context.Users.Where(user => user.Login == HttpContext.User.Identity.Name).ToList().Last();
                encryptHelper = new EncryptHelper(CurrentUser, HashAlgorithm);
                Account.User = CurrentUser;
                Account.Password = encryptHelper.Encrypt(Account.Password);
                _context.Accounts.Add(Account);
                
                await _context.SaveChangesAsync();
                return RedirectToPage("Passwords");
            }
            return Page();
        }
    }
}