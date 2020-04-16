using System;
using System.Collections.Generic;

namespace MM.Models
{
    public partial class AccountDAO
    {
        public AccountDAO()
        {
            AccountFoodFavorites = new HashSet<AccountFoodFavoriteDAO>();
            Notificaitons = new HashSet<NotificaitonDAO>();
            Orders = new HashSet<OrderDAO>();
        }

        public long Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string PasswordRecoveryCode { get; set; }
        public DateTime? ExpiredTimeCode { get; set; }
        public string Address { get; set; }
        public DateTime? Dob { get; set; }
        public string Avatar { get; set; }
        public long RoleId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<AccountFoodFavoriteDAO> AccountFoodFavorites { get; set; }
        public virtual ICollection<NotificaitonDAO> Notificaitons { get; set; }
        public virtual ICollection<OrderDAO> Orders { get; set; }
    }
}
