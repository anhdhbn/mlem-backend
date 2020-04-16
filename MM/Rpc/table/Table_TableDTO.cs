using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using MM.Entities;

namespace MM.Rpc.table
{
    public class Table_TableDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long StatusId { get; set; }
        public long? OrderId { get; set; }
        public Table_OrderDTO Order { get; set; }
        public Table_TableDTO() {}
        public Table_TableDTO(Table Table)
        {
            this.Id = Table.Id;
            this.Code = Table.Code;
            this.StatusId = Table.StatusId;
            this.OrderId = Table.OrderId;
            this.Order = Table.Order == null ? null : new Table_OrderDTO(Table.Order);
            this.Errors = Table.Errors;
        }
    }

    public class Table_TableFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter OrderId { get; set; }
        public TableOrder OrderBy { get; set; }
    }
}
