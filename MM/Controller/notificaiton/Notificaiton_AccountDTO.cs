using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using MM.Entities;

namespace MM.Controller.notificaiton
{
    public class Notificaiton_AccountDTO : DataDTO
    {
        
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
        
        public long? SexId { get; set; }
        
        public long StatusId { get; set; }
        
        public long RoleId { get; set; }
        

        public Notificaiton_AccountDTO() {}
        public Notificaiton_AccountDTO(Account Account)
        {
            
            this.Id = Account.Id;
            
            this.DisplayName = Account.DisplayName;
            
            this.Email = Account.Email;
            
            this.Phone = Account.Phone;
            
            this.Password = Account.Password;
            
            this.Salt = Account.Salt;
            
            this.PasswordRecoveryCode = Account.PasswordRecoveryCode;
            
            this.ExpiredTimeCode = Account.ExpiredTimeCode;
            
            this.Address = Account.Address;
            
            this.Dob = Account.Dob;
            
            this.Avatar = Account.Avatar;
            
            
            this.RoleId = Account.RoleId;
            
            this.Errors = Account.Errors;
        }
    }

    public class Notificaiton_AccountFilterDTO : FilterDTO
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
        
        public AccountOrder OrderBy { get; set; }
    }
}