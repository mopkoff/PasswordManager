using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Model
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Заполните поле Логин")]
        //MinLength(4, ErrorMessage = "Поле Логин должно содержать не менее 4 символов"),
        // StringLength(20, ErrorMessage = "Поле Логин должно содержать не более 20 символов")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Заполните поле Пароль")]
        //MinLength(8, ErrorMessage ="Поле Пароль должно содержать не менее 8 символов"), 
        //StringLength(30, ErrorMessage = "Поле Пароль должно содержать не более 30 символов")]        
        public string Password { get; set; }


        [Required(ErrorMessage = "Заполните поле Сервис")]
        // MinLength(3, ErrorMessage = "Поле Сервис должно содержать не менее 3 символов"),
        // StringLength(100, ErrorMessage = "Поле Сервис должно содержать не более 100 символов")]
        public string ServiceName { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
