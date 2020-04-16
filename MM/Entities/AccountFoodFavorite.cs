using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MM.Entities
{
    public class AccountFoodFavorite : DataEntity,  IEquatable<AccountFoodFavorite>
    {
        public long AccountId { get; set; }
        public long FoodId { get; set; }
        public Account Account { get; set; }
        public Food Food { get; set; }

        public bool Equals(AccountFoodFavorite other)
        {
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class AccountFoodFavoriteFilter : FilterEntity
    {
        public IdFilter AccountId { get; set; }
        public IdFilter FoodId { get; set; }
        public List<AccountFoodFavoriteFilter> OrFilter { get; set; }
        public AccountFoodFavoriteOrder OrderBy {get; set;}
        public AccountFoodFavoriteSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AccountFoodFavoriteOrder
    {
        Account = 0,
        Food = 1,
    }

    [Flags]
    public enum AccountFoodFavoriteSelect:long
    {
        ALL = E.ALL,
        Account = E._0,
        Food = E._1,
    }
}
