using System;
using System.Collections.Generic;

namespace MM.Models
{
    public partial class FoodTypeDAO
    {
        public FoodTypeDAO()
        {
            FoodFoodTypeMappings = new HashSet<FoodFoodTypeMappingDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long StatusId { get; set; }

        public virtual ICollection<FoodFoodTypeMappingDAO> FoodFoodTypeMappings { get; set; }
    }
}
