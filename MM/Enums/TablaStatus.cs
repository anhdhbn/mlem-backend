using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MM.Enums
{
    public class TablaStatus
    {
        public static GenericEnum BUSY = new GenericEnum { Id = 1, Code = "Busy", Name = "Đang bận" };
        public static GenericEnum EMPTY = new GenericEnum { Id = 2, Code = "Empty", Name = "Trống" };
    }
}
