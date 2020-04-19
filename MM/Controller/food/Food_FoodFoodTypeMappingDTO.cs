using Common;
using MM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MM.Controller.food
{
    public class Food_FoodFoodTypeMappingDTO : DataDTO
    {
        public long Id { get; set; }
        public long FoodId { get; set; }
        public long FoodTypeId { get; set; }
        public Food_FoodDTO Food { get; set; }
        public Food_FoodTypeDTO FoodType { get; set; }
        public Food_FoodFoodTypeMappingDTO() { }
        public Food_FoodFoodTypeMappingDTO(FoodFoodTypeMapping FoodFoodTypeMapping)
        {
            this.Id = FoodFoodTypeMapping.Id;
            this.FoodId = FoodFoodTypeMapping.FoodId;
            this.FoodTypeId = FoodFoodTypeMapping.FoodTypeId;
            this.Food = FoodFoodTypeMapping.Food == null ? null : new Food_FoodDTO(FoodFoodTypeMapping.Food);
            this.FoodType = FoodFoodTypeMapping.FoodType == null ? null : new Food_FoodTypeDTO(FoodFoodTypeMapping.FoodType);
            this.Errors = FoodFoodTypeMapping.Errors;
        }
    }

    public class Food_FoodFoodTypeMappingFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter FoodId { get; set; }
        public IdFilter FoodTypeId { get; set; }
        public FoodFoodTypeMappingOrder OrderBy { get; set; }
    }
}
