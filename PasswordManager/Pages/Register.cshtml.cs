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
                ModelState.AddModelError("", "������������ � ����� ������ ��� ���������������");
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
            [Required(ErrorMessage = "��������� ���� �����"),
            MinLength(4, ErrorMessage = "���� ����� ������ ��������� �� ����� 8 ��������"),
            StringLength(20, ErrorMessage = "���� ����� ������ ��������� �� ����� 20 ��������")]
            public string Username { get; set; }

            [Required(ErrorMessage = "��������� ���� ������"),
            MinLength(8, ErrorMessage = "���� ������ ������ ��������� �� ����� 8 ��������"),
            StringLength(30, ErrorMessage = "���� ������ ������ ��������� �� ����� 30 ��������")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Compare("Password", ErrorMessage = "������ �� ���������!")]
            [DataType(DataType.Password)]
            public string PasswordConfirm { get; set; }
        }
    }
}