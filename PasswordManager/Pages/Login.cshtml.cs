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
        private string[] Roles;
        
        public LoginModel(ApplicationContext _context, IConfiguration _configuration)
        {
            CookieExpirationTimeMin = 20;
            configuration = _configuration;
            context = _context;
            HashAlgorithm =
                configuration.GetValue<string>("HashAlgorithm");
            Roles = configuration.GetValue<string>("Roles").Split(configuration.GetValue<string>("RolesSeparator"));
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var loggingUser = context.Users.Where(u => (u.Login == loginData.Username)).ToList();
                if ((loggingUser.Count != 1) || !(HashHelper.VerifyHash(loginData.Password, HashAlgorithm, loggingUser[0].Password)))
                {
                    ModelState.AddModelError("ErrorLogin", "Неверный логин или пароль");
                    return Page();
                }
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, loginData.Username));
                identity.AddClaim(new Claim(ClaimTypes.Name, loginData.Username));
                if (loggingUser[0].Login != configuration.GetValue<string>("AdminName"))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, Roles[0]));
                }
                else
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, Roles[1]));
                }
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
                ModelState.AddModelError("ErrorLogin", "Введите логин и пароль");
                return Page();
            }
        }

        public class LoginData
        {
            [Required(ErrorMessage = " ")]
            public string Username { get; set; }

            [Required(ErrorMessage = " ")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            public bool RememberMe { get; set; }
        }
    }
}