using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MM.Enums
{
    public class AccountRole
    {
        public static GenericEnum ADMIN = new GenericEnum { Id = 1, Code = "Admin", Name = "Chủ nhà hàng" };
        public static GenericEnum USER = new GenericEnum { Id = 2, Code = "User", Name = "Khách hàng" };
    }
}
