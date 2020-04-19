using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using MM.Entities;

namespace MM.Controller.order_content
{
    public class OrderContent_FoodFoodTypeMappingDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public long FoodId { get; set; }
        
        public long FoodTypeId { get; set; }
        

        public OrderContent_FoodFoodTypeMappingDTO() {}
        public OrderContent_FoodFoodTypeMappingDTO(FoodFoodTypeMapping FoodFoodTypeMapping)
        {
            
            this.Id = FoodFoodTypeMapping.Id;
            
            this.FoodId = FoodFoodTypeMapping.FoodId;
            
            this.FoodTypeId = FoodFoodTypeMapping.FoodTypeId;
            
            this.Errors = FoodFoodTypeMapping.Errors;
        }
    }

    public class OrderContent_FoodFoodTypeMappingFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter FoodId { get; set; }
        
        public IdFilter FoodTypeId { get; set; }
        
        public FoodFoodTypeMappingOrder OrderBy { get; set; }
    }
}