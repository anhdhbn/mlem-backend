using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MM.Enums
{
    public class OrderStatus
    {
        public static GenericEnum NEW = new GenericEnum { Id = 1, Code = "New", Name = "Mới tạo" };
        public static GenericEnum PENDING = new GenericEnum { Id = 1, Code = "Pending", Name = "Đã đặt" };
        public static GenericEnum APPROVED = new GenericEnum { Id = 2, Code = "Approved", Name = "Đã xác nhận" };
        public static GenericEnum REJECTED = new GenericEnum { Id = 2, Code = "Rejected", Name = "Đã từ chối" };
        public static GenericEnum DONE = new GenericEnum { Id = 2, Code = "Done", Name = "Đã thanh toán" };
    }
}
