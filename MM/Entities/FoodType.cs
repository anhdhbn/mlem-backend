using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MM.Entities
{
    public class FoodType : DataEntity,  IEquatable<FoodType>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long StatusId { get; set; }

        public bool Equals(FoodType other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class FoodTypeFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter StatusId { get; set; }
        public List<FoodTypeFilter> OrFilter { get; set; }
        public FoodTypeOrder OrderBy {get; set;}
        public FoodTypeSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FoodTypeOrder
    {
        Id = 0,
        Name = 1,
        Status = 2,
    }

    [Flags]
    public enum FoodTypeSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Name = E._1,
        Status = E._2,
    }
}
