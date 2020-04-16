using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MM.Entities
{
    public class FoodFoodTypeMapping : DataEntity,  IEquatable<FoodFoodTypeMapping>
    {
        public long Id { get; set; }
        public long FoodId { get; set; }
        public long FoodTypeId { get; set; }
        public Food Food { get; set; }
        public FoodType FoodType { get; set; }

        public bool Equals(FoodFoodTypeMapping other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class FoodFoodTypeMappingFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter FoodId { get; set; }
        public IdFilter FoodTypeId { get; set; }
        public List<FoodFoodTypeMappingFilter> OrFilter { get; set; }
        public FoodFoodTypeMappingOrder OrderBy {get; set;}
        public FoodFoodTypeMappingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FoodFoodTypeMappingOrder
    {
        Id = 0,
        Food = 1,
        FoodType = 2,
    }

    [Flags]
    public enum FoodFoodTypeMappingSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Food = E._1,
        FoodType = E._2,
    }
}
