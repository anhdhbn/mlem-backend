using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using MM.Entities;

namespace MM.Controller.order_content
{
    public class OrderContent_OrderDTO : DataDTO
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
        

        public OrderContent_OrderDTO() {}
        public OrderContent_OrderDTO(Order Order)
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
            
            this.Errors = Order.Errors;
        }
    }

    public class OrderContent_OrderFilterDTO : FilterDTO
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