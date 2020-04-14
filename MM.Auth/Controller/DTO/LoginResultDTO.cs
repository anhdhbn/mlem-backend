using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MM.Auth.Controller.DTO
{
    public class LoginResultDTO
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Token { get; set; }
        public DateTime? ExpiredTime { get; set; }
        public bool RoleId { get; set; }
    }
}
