using System;
using System.Collections.Generic;

namespace MM.Models
{
    public partial class FoodFoodGroupingMappingDAO
    {
        public long FoodId { get; set; }
        public long FoodGroupingId { get; set; }

        public virtual FoodDAO Food { get; set; }
        public virtual FoodGroupingDAO FoodGrouping { get; set; }
    }
}
