using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Заполните поле Логин"),
        MinLength(4, ErrorMessage = "Поле Логин должно содержать не менее 4 символов"),
        StringLength(20, ErrorMessage = "Поле Логин должно содержать не более 20 символов")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Заполните поле Пароль"),
        MinLength(8, ErrorMessage = "Поле Пароль должно содержать не менее 8 символов"),
        StringLength(30, ErrorMessage = "Поле Пароль должно содержать не более 30 символов")]
        public string Password { get; set; }
    }
}
