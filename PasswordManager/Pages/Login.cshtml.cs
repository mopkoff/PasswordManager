using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using PasswordManager.Model;
using PasswordManager.Helper;
using System.Linq;
using System;
using Microsoft.Extensions.Configuration;

namespace PasswordManager.Pages
{
    
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginData loginData { get; set; }
        private readonly ApplicationContext context;
        private readonly int CookieExpirationTimeMin;
        private IConfiguration configuration;
        public string HashAlgorithm;

        public LoginModel(ApplicationContext _context, IConfiguration _configuration)
        {
            CookieExpirationTimeMin = 20;
            configuration = _configuration;
            context = _context;
            HashAlgorithm =
                configuration.GetValue<string>("HashAlgorithm");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var isRegistered = context.Users.Where(u => (u.Login == loginData.Username)).ToList();
                if ((isRegistered.Count != 1) || !(HashHelper.VerifyHash(loginData.Password, HashAlgorithm, isRegistered[0].Password)))
                {
                    ModelState.AddModelError("", "Неверный логин или пароль");
                    return Page();
                }
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, loginData.Username));
                identity.AddClaim(new Claim(ClaimTypes.Name, loginData.Username));

                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                {
                    IsPersistent = loginData.RememberMe,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(CookieExpirationTimeMin)
                    
                });

                return RedirectToPage("Index");
            }
            else
            {
                ModelState.AddModelError("", "Введите логин и пароль");
                return Page();
            }
        }

        public class LoginData
        {
            [Required]
            public string Username { get; set; }

            [Required, DataType(DataType.Password)]
            public string Password { get; set; }

            public bool RememberMe { get; set; }
        }
    }
}