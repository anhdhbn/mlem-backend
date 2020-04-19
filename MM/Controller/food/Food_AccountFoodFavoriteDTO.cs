using Common;
using MM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MM.Controller.food
{
    public class Food_AccountFoodFavoriteDTO : DataDTO
    {
        public long AccountId { get; set; }
        public long FoodId { get; set; }
        public Food_FoodDTO Food { get; set; }
        public Food_AccountFoodFavoriteDTO() { }
        public Food_AccountFoodFavoriteDTO(AccountFoodFavorite AccountFoodFavorite)
        {
            this.AccountId = AccountFoodFavorite.AccountId;
            this.FoodId = AccountFoodFavorite.FoodId;
            this.Food = AccountFoodFavorite.Food == null ? null : new Food_FoodDTO(AccountFoodFavorite.Food);
            this.Errors = AccountFoodFavorite.Errors;
        }
    }

    public class Food_AccountFoodFavoriteFilterDTO : FilterDTO
    {
        public IdFilter AccountId { get; set; }
        public IdFilter FoodId { get; set; }
        public AccountFoodFavoriteOrder OrderBy { get; set; }
    }

}
