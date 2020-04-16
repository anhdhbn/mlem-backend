using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using MM.Entities;

namespace MM.Rpc.notificaiton
{
    public class Notificaiton_NotificaitonDTO : DataDTO
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }
        public bool Unread { get; set; }
        public Notificaiton_AccountDTO Account { get; set; }
        public Notificaiton_NotificaitonDTO() {}
        public Notificaiton_NotificaitonDTO(Notificaiton Notificaiton)
        {
            this.Id = Notificaiton.Id;
            this.AccountId = Notificaiton.AccountId;
            this.Content = Notificaiton.Content;
            this.Time = Notificaiton.Time;
            this.Unread = Notificaiton.Unread;
            this.Account = Notificaiton.Account == null ? null : new Notificaiton_AccountDTO(Notificaiton.Account);
            this.Errors = Notificaiton.Errors;
        }
    }

    public class Notificaiton_NotificaitonFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter AccountId { get; set; }
        public StringFilter Content { get; set; }
        public DateFilter Time { get; set; }
        public NotificaitonOrder OrderBy { get; set; }
    }
}
