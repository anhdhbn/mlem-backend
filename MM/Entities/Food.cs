using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MM.Entities
{
    public class Food : DataEntity,  IEquatable<Food>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal PriceEach { get; set; }
        public decimal? DiscountRate { get; set; }
        public string Image { get; set; }
        public long StatusId { get; set; }
        public string Descreption { get; set; }

        public bool Equals(Food other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class FoodFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public DecimalFilter PriceEach { get; set; }
        public DecimalFilter DiscountRate { get; set; }
        public StringFilter Image { get; set; }
        public IdFilter StatusId { get; set; }
        public StringFilter Descreption { get; set; }
        public List<FoodFilter> OrFilter { get; set; }
        public FoodOrder OrderBy {get; set;}
        public FoodSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FoodOrder
    {
        Id = 0,
        Name = 1,
        PriceEach = 2,
        DiscountRate = 3,
        Image = 4,
        Status = 5,
        Descreption = 6,
    }

    [Flags]
    public enum FoodSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Name = E._1,
        PriceEach = E._2,
        DiscountRate = E._3,
        Image = E._4,
        Status = E._5,
        Descreption = E._6,
    }
}
