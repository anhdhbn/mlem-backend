using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MM.Entities
{
    public class Notificaiton : DataEntity,  IEquatable<Notificaiton>
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }
        public bool Unread { get; set; }
        public Account Account { get; set; }

        public bool Equals(Notificaiton other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class NotificaitonFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter AccountId { get; set; }
        public StringFilter Content { get; set; }
        public DateFilter Time { get; set; }
        public List<NotificaitonFilter> OrFilter { get; set; }
        public NotificaitonOrder OrderBy {get; set;}
        public NotificaitonSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum NotificaitonOrder
    {
        Id = 0,
        Account = 1,
        Content = 2,
        Time = 3,
        Unread = 4,
    }

    [Flags]
    public enum NotificaitonSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Account = E._1,
        Content = E._2,
        Time = E._3,
        Unread = E._4,
    }
}
