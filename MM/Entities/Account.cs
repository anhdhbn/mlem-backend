using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MM.Entities
{
    public class Account : DataEntity,  IEquatable<Account>
    {
        public long Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string Token { get; set; }
        public string Salt { get; set; }
        public string PasswordRecoveryCode { get; set; }
        public DateTime? ExpiredTimeCode { get; set; }
        public string Address { get; set; }
        public DateTime? Dob { get; set; }
        public string Avatar { get; set; }
        public long RoleId { get; set; }
        public List<AccountFoodFavorite> AccountFoodFavorites { get; set; }
        public List<Order> Orders { get; set; }

        public bool Equals(Account other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class AccountFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter DisplayName { get; set; }
        public StringFilter Email { get; set; }
        public StringFilter Phone { get; set; }
        public StringFilter Password { get; set; }
        public StringFilter Salt { get; set; }
        public StringFilter PasswordRecoveryCode { get; set; }
        public DateFilter ExpiredTimeCode { get; set; }
        public StringFilter Address { get; set; }
        public DateFilter Dob { get; set; }
        public StringFilter Avatar { get; set; }
        public IdFilter RoleId { get; set; }
        public List<AccountFilter> OrFilter { get; set; }
        public AccountOrder OrderBy {get; set;}
        public AccountSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AccountOrder
    {
        Id = 0,
        DisplayName = 1,
        Email = 2,
        Phone = 3,
        Password = 4,
        Salt = 5,
        PasswordRecoveryCode = 6,
        ExpiredTimeCode = 7,
        Address = 8,
        Dob = 9,
        Avatar = 10,
        Role = 11,
    }

    [Flags]
    public enum AccountSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        DisplayName = E._1,
        Email = E._2,
        Phone = E._3,
        Password = E._4,
        Salt = E._5,
        PasswordRecoveryCode = E._6,
        ExpiredTimeCode = E._7,
        Address = E._8,
        Dob = E._9,
        Avatar = E._10,
        Role = E._11,
    }
}
