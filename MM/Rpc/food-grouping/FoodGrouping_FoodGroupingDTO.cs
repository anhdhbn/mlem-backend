using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using MM.Entities;

namespace MM.Rpc.food_grouping
{
    public class FoodGrouping_FoodGroupingDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long StatusId { get; set; }
        public FoodGrouping_FoodGroupingDTO() {}
        public FoodGrouping_FoodGroupingDTO(FoodGrouping FoodGrouping)
        {
            this.Id = FoodGrouping.Id;
            this.Name = FoodGrouping.Name;
            this.StatusId = FoodGrouping.StatusId;
            this.Errors = FoodGrouping.Errors;
        }
    }

    public class FoodGrouping_FoodGroupingFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter StatusId { get; set; }
        public FoodGroupingOrder OrderBy { get; set; }
    }
}
