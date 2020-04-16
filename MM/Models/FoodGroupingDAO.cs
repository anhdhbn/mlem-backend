using System;
using System.Collections.Generic;

namespace MM.Models
{
    public partial class FoodGroupingDAO
    {
        public FoodGroupingDAO()
        {
            FoodFoodGroupingMappings = new HashSet<FoodFoodGroupingMappingDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long StatusId { get; set; }

        public virtual ICollection<FoodFoodGroupingMappingDAO> FoodFoodGroupingMappings { get; set; }
    }
}
