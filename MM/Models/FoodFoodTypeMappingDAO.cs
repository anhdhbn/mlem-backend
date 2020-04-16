using System;
using System.Collections.Generic;

namespace MM.Models
{
    public partial class FoodFoodTypeMappingDAO
    {
        public FoodFoodTypeMappingDAO()
        {
            OrderContents = new HashSet<OrderContentDAO>();
        }

        public long Id { get; set; }
        public long FoodId { get; set; }
        public long FoodTypeId { get; set; }

        public virtual FoodDAO Food { get; set; }
        public virtual FoodTypeDAO FoodType { get; set; }
        public virtual ICollection<OrderContentDAO> OrderContents { get; set; }
    }
}
