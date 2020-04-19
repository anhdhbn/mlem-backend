using Common;
using MM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MM.Controller.food
{
    public class Food_FoodTypeDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long StatusId { get; set; }
        public Food_FoodTypeDTO() { }
        public Food_FoodTypeDTO(FoodType FoodType)
        {
            this.Id = FoodType.Id;
            this.Name = FoodType.Name;
            this.StatusId = FoodType.StatusId;
            this.Errors = FoodType.Errors;
        }
    }

    public class Food_FoodTypeFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter StatusId { get; set; }
        public FoodTypeOrder OrderBy { get; set; }
    }
}
