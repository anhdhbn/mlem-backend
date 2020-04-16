using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MM.Entities
{
    public class FoodGrouping : DataEntity,  IEquatable<FoodGrouping>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long StatusId { get; set; }

        public bool Equals(FoodGrouping other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class FoodGroupingFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter StatusId { get; set; }
        public List<FoodGroupingFilter> OrFilter { get; set; }
        public FoodGroupingOrder OrderBy {get; set;}
        public FoodGroupingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FoodGroupingOrder
    {
        Id = 0,
        Name = 1,
        Status = 2,
    }

    [Flags]
    public enum FoodGroupingSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Name = E._1,
        Status = E._2,
    }
}
