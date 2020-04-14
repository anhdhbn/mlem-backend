using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MM.Auth.Controller.DTO
{
    public class AccountDTO
    {
        public long Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime Dob { get; set; }
        public long SexId {get;set;}
        public string Sex { get; set; }
        public long StatusId { get; set; }
        public string Status { get; set; }
        public long RoleId { get; set; }
    }
}
