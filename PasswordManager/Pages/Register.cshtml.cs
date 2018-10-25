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

namespace PasswordManager.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public RegisterData registerData { get; set; }
        private readonly ApplicationContext _context;

        public RegisterModel(ApplicationContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var isRegistered = _context.Users.Where(u => (u.Login == registerData.Username)).ToList();
            if (isRegistered.Count == 1)
            {
                ModelState.AddModelError("", "Пользователь с таким именем уже зарегистрирован");
                return Page();
            }
            if (ModelState.IsValid)
            {
                _context.Users.Add(new User()
                {
                    Login = registerData.Username,
                    Password = HashHelper.ComputeHash(registerData.Password, "SHA512", null)
                });
                await _context.SaveChangesAsync();
                return RedirectToPage("Login");
            }
            return Page();
        }

        public class RegisterData
        {
            [Required(ErrorMessage = "Заполните поле Логин"),
            MinLength(4, ErrorMessage = "Поле Логин должно содержать не менее 8 символов"),
            StringLength(20, ErrorMessage = "Поле Логин должно содержать не более 20 символов")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Заполните поле Пароль"),
            MinLength(8, ErrorMessage = "Поле Пароль должно содержать не менее 8 символов"),
            StringLength(30, ErrorMessage = "Поле Пароль должно содержать не более 30 символов")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Compare("Password", ErrorMessage = "Пароли не совпадают!")]
            [DataType(DataType.Password)]
            public string PasswordConfirm { get; set; }
        }
    }
}