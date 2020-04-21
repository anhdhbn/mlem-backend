using Common;
using MM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MM.Controller.account
{
    public class Account_AccountFoodFavoriteDTO : DataDTO
    {
        public long AccountId { get; set; }
        public long FoodId { get; set; }
        public Account_AccountFoodFavoriteDTO() { }
        public Account_AccountFoodFavoriteDTO(AccountFoodFavorite AccountFoodFavorite)
        {
            this.AccountId = AccountFoodFavorite.AccountId;
            this.FoodId = AccountFoodFavorite.FoodId;
            this.Errors = AccountFoodFavorite.Errors;
        }
    }

    public class Account_AccountFoodFavoriteFilterDTO : FilterDTO
    {
        public IdFilter AccountId { get; set; }
        public IdFilter FoodId { get; set; }
        public AccountFoodFavoriteOrder OrderBy { get; set; }
    }
}
