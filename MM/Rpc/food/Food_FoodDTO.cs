using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using MM.Entities;

namespace MM.Rpc.food
{
    public class Food_FoodDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal PriceEach { get; set; }
        public decimal? DiscountRate { get; set; }
        public string Image { get; set; }
        public long StatusId { get; set; }
        public string Descreption { get; set; }
        public Food_FoodDTO() {}
        public Food_FoodDTO(Food Food)
        {
            this.Id = Food.Id;
            this.Name = Food.Name;
            this.PriceEach = Food.PriceEach;
            this.DiscountRate = Food.DiscountRate;
            this.Image = Food.Image;
            this.StatusId = Food.StatusId;
            this.Descreption = Food.Descreption;
            this.Errors = Food.Errors;
        }
    }

    public class Food_FoodFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public DecimalFilter PriceEach { get; set; }
        public DecimalFilter DiscountRate { get; set; }
        public StringFilter Image { get; set; }
        public IdFilter StatusId { get; set; }
        public StringFilter Descreption { get; set; }
        public FoodOrder OrderBy { get; set; }
    }
}
