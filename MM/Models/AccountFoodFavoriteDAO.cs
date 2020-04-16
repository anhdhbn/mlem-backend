using System;
using System.Collections.Generic;

namespace MM.Models
{
    public partial class AccountFoodFavoriteDAO
    {
        public long AccountId { get; set; }
        public long FoodId { get; set; }

        public virtual AccountDAO Account { get; set; }
        public virtual FoodDAO Food { get; set; }
    }
}
