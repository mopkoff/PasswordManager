using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Helper
{
    public class FailedAccessAttemptCounter
    {

        [Key]
        public string Login { get; set; }

        public DateTime LastFailedAccessAttempt { get; set; }

        public int FailedAccessAttemptCount { get; set; }

    }
       
}
