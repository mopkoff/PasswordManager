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
using Microsoft.EntityFrameworkCore;

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
        int DefaultLockoutTimeSpanMin;
        int MaxFailedAccessAttempts;

        public LoginModel(ApplicationContext _context, IConfiguration _configuration)
        {
            CookieExpirationTimeMin = 20;
            configuration = _configuration;
            context = _context;
            HashAlgorithm =
                configuration.GetValue<string>("HashAlgorithm");
            Roles = configuration.GetValue<string>("Roles").Split(configuration.GetValue<string>("RolesSeparator"));

            DefaultLockoutTimeSpanMin = configuration.GetValue<int>("DefaultLockoutTimeSpanMin");
            MaxFailedAccessAttempts = configuration.GetValue<int>("MaxFailedAccessAttempts");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var loggingUser = context.Users.Where(u => (u.Login == loginData.Username)).ToList();
                var FAAClist = context.FailedAccessAttemptCounters.Where(faac => (faac.Login == loginData.Username)).ToList();
                var FAAC = FAAClist.Count == 0 ? null : FAAClist[0];
                TimeSpan MinutesLeft;
                if (FAAC != null) {
                    MinutesLeft = FAAC.LastFailedAccessAttempt.AddMinutes(DefaultLockoutTimeSpanMin).Subtract(DateTime.Now); 
                    if (MinutesLeft.Minutes > 30)
                        FAAC.FailedAccessAttemptCount = 0;
                    else if (FAAC.FailedAccessAttemptCount > MaxFailedAccessAttempts)
                    {
                        ModelState.AddModelError("ErrorLogin", "Превышено количество попыток входа. Осталось минут до конца блокировки: " + MinutesLeft.Minutes);
                        return Page();
                    }
                }

                if ((loggingUser.Count != 1) || !(HashHelper.VerifyHash(loginData.Password, HashAlgorithm, loggingUser[0].Password)))
                {
                    if (FAAC == null)
                    {
                        context.FailedAccessAttemptCounters.Add(new FailedAccessAttemptCounter
                        {
                            Login = loginData.Username,
                            FailedAccessAttemptCount = 1,
                            LastFailedAccessAttempt = DateTime.Now
                        });
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        FAAC.FailedAccessAttemptCount++;
                        FAAC.LastFailedAccessAttempt = DateTime.Now;
                        context.Attach(FAAC).State = EntityState.Modified;
                        await context.SaveChangesAsync();
                    }
                    ModelState.AddModelError("ErrorLogin", "Неверный логин или пароль. У вас осталось "  + (MaxFailedAccessAttempts - FAAC.FailedAccessAttemptCount) + " попытки.");
                    return Page();
                }
                if (FAAC != null)
                {
                    FAAC.FailedAccessAttemptCount = 0;
                    context.FailedAccessAttemptCounters.Update(FAAC);
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