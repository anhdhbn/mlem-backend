using System;
using System.Collections.Generic;

namespace MM.Models
{
    public partial class FoodDAO
    {
        public FoodDAO()
        {
            AccountFoodFavorites = new HashSet<AccountFoodFavoriteDAO>();
            FoodFoodGroupingMappings = new HashSet<FoodFoodGroupingMappingDAO>();
            FoodFoodTypeMappings = new HashSet<FoodFoodTypeMappingDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public decimal PriceEach { get; set; }
        public decimal? DiscountRate { get; set; }
        public string Image { get; set; }
        public long StatusId { get; set; }
        public string Descreption { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<AccountFoodFavoriteDAO> AccountFoodFavorites { get; set; }
        public virtual ICollection<FoodFoodGroupingMappingDAO> FoodFoodGroupingMappings { get; set; }
        public virtual ICollection<FoodFoodTypeMappingDAO> FoodFoodTypeMappings { get; set; }
    }
}
