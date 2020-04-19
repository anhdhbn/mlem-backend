using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using MM.Entities;

namespace MM.Controller.order
{
    public class Order_OrderDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? PayDate { get; set; }
        public long AccountId { get; set; }
        public long NumOfTable { get; set; }
        public long NumOfPerson { get; set; }
        public string Descreption { get; set; }
        public long StatusId { get; set; }
        public Order_AccountDTO Account { get; set; }
        public Order_OrderDTO() {}
        public Order_OrderDTO(Order Order)
        {
            this.Id = Order.Id;
            this.Code = Order.Code;
            this.OrderDate = Order.OrderDate;
            this.PayDate = Order.PayDate;
            this.AccountId = Order.AccountId;
            this.NumOfTable = Order.NumOfTable;
            this.NumOfPerson = Order.NumOfPerson;
            this.Descreption = Order.Descreption;
            this.StatusId = Order.StatusId;
            this.Account = Order.Account == null ? null : new Order_AccountDTO(Order.Account);
            this.Errors = Order.Errors;
        }
    }

    public class Order_OrderFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public DateFilter OrderDate { get; set; }
        public DateFilter PayDate { get; set; }
        public IdFilter AccountId { get; set; }
        public LongFilter NumOfTable { get; set; }
        public LongFilter NumOfPerson { get; set; }
        public StringFilter Descreption { get; set; }
        public IdFilter StatusId { get; set; }
        public OrderOrder OrderBy { get; set; }
    }
}
