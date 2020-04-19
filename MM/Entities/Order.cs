using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MM.Entities
{
    public class Order : DataEntity,  IEquatable<Order>
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
        public Account Account { get; set; }
        public List<OrderContent> OrderContents { get; set; }
        public List<Table> Tables { get; set; }

        public bool Equals(Order other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class OrderFilter : FilterEntity
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
        public List<OrderFilter> OrFilter { get; set; }
        public OrderOrder OrderBy {get; set;}
        public OrderSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrderOrder
    {
        Id = 0,
        Code = 1,
        OrderDate = 2,
        PayDate = 3,
        Account = 4,
        NumOfTable = 5,
        NumOfPerson = 6,
        Descreption = 7,
        Status = 8,
    }

    [Flags]
    public enum OrderSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        OrderDate = E._2,
        PayDate = E._3,
        Account = E._4,
        NumOfTable = E._5,
        NumOfPerson = E._6,
        Descreption = E._7,
        Status = E._8,
        OrderContents = E._9,
        Tables = E._10,
    }
}
