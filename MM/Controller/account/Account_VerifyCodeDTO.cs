using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MM.Controller.account
{
    public class Account_VerifyCodeDTO
    {
        public long Id { get; set; }
        public string PasswordRecoveryCode { get; set; }
    }
}
