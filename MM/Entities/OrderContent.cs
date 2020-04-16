using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MM.Entities
{
    public class OrderContent : DataEntity,  IEquatable<OrderContent>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long OrderId { get; set; }
        public long FoodFoodTypeMappingId { get; set; }
        public long Quantity { get; set; }
        public long StatusId { get; set; }
        public FoodFoodTypeMapping FoodFoodTypeMapping { get; set; }
        public Order Order { get; set; }

        public bool Equals(OrderContent other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class OrderContentFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public IdFilter OrderId { get; set; }
        public IdFilter FoodFoodTypeMappingId { get; set; }
        public LongFilter Quantity { get; set; }
        public IdFilter StatusId { get; set; }
        public List<OrderContentFilter> OrFilter { get; set; }
        public OrderContentOrder OrderBy {get; set;}
        public OrderContentSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrderContentOrder
    {
        Id = 0,
        Code = 1,
        Order = 2,
        FoodFoodTypeMapping = 3,
        Quantity = 4,
        Status = 5,
    }

    [Flags]
    public enum OrderContentSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Order = E._2,
        FoodFoodTypeMapping = E._3,
        Quantity = E._4,
        Status = E._5,
    }
}
