using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Model
{
    public class FailedAccessAttemptCounter
    {

        [Key]
        public int Id { get; set; }

        public string Login { get; set; }

        public DateTime LastFailedAccessAttempt { get; set; }

        public int FailedAccessAttemptCount { get; set; }

    }
       
}
