using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MM.Enums
{
    public class FoodStatus
    {
        public static GenericEnum ACTIVE = new GenericEnum { Id = 1, Code = "Active", Name = "Đang bán" };
        public static GenericEnum INACTIVE = new GenericEnum { Id = 2, Code = "Inactive", Name = "Dừng bán" };
    }
}
