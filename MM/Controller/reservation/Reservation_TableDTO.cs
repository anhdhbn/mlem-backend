using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using MM.Entities;

namespace MM.Controller.reservation
{
    public class Reservation_TableDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public Reservation_TableDTO() {}
        public Reservation_TableDTO(Table Table)
        {
            this.Id = Table.Id;
            this.Code = Table.Code;
            this.Errors = Table.Errors;
        }
    }

    public class Reservation_TableFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public TableOrder OrderBy { get; set; }
    }
}