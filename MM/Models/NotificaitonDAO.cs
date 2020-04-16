using System;
using System.Collections.Generic;

namespace MM.Models
{
    public partial class NotificaitonDAO
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }
        public bool Unread { get; set; }

        public virtual AccountDAO Account { get; set; }
    }
}
