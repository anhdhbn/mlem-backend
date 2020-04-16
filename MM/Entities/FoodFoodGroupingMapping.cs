using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MM.Entities
{
    public class FoodFoodGroupingMapping : DataEntity,  IEquatable<FoodFoodGroupingMapping>
    {
        public long FoodId { get; set; }
        public long FoodGroupingId { get; set; }
        public Food Food { get; set; }
        public FoodGrouping FoodGrouping { get; set; }

        public bool Equals(FoodFoodGroupingMapping other)
        {
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class FoodFoodGroupingMappingFilter : FilterEntity
    {
        public IdFilter FoodId { get; set; }
        public IdFilter FoodGroupingId { get; set; }
        public List<FoodFoodGroupingMappingFilter> OrFilter { get; set; }
        public FoodFoodGroupingMappingOrder OrderBy {get; set;}
        public FoodFoodGroupingMappingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FoodFoodGroupingMappingOrder
    {
        Food = 0,
        FoodGrouping = 1,
    }

    [Flags]
    public enum FoodFoodGroupingMappingSelect:long
    {
        ALL = E.ALL,
        Food = E._0,
        FoodGrouping = E._1,
    }
}
