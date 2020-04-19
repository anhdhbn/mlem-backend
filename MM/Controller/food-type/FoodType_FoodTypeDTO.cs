using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using MM.Entities;

namespace MM.Controller.food_type
{
    public class FoodType_FoodTypeDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long StatusId { get; set; }
        public FoodType_FoodTypeDTO() {}
        public FoodType_FoodTypeDTO(FoodType FoodType)
        {
            this.Id = FoodType.Id;
            this.Name = FoodType.Name;
            this.StatusId = FoodType.StatusId;
            this.Errors = FoodType.Errors;
        }
    }

    public class FoodType_FoodTypeFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter StatusId { get; set; }
        public FoodTypeOrder OrderBy { get; set; }
    }
}
