using System;
using System.Collections.Generic;

namespace MM.Models
{
    public partial class OrderContentDAO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long OrderId { get; set; }
        public long FoodFoodTypeMappingId { get; set; }
        public long Quantity { get; set; }
        public long StatusId { get; set; }

        public virtual FoodFoodTypeMappingDAO FoodFoodTypeMapping { get; set; }
        public virtual OrderDAO Order { get; set; }
    }
}
