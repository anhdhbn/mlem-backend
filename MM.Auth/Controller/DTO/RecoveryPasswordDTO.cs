using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MM.Auth.Controller.DTO
{
    public class RecoveryPasswordDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
