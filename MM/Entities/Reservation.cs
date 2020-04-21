using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MM.Entities
{
    public class Reservation : DataEntity,  IEquatable<Reservation>
    {
        public long Id { get; set; }
        public long TableId { get; set; }
        public DateTime Date { get; set; }
        public long StatusId { get; set; }
        public Table Table { get; set; }

        public bool Equals(Reservation other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class ReservationFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter TableId { get; set; }
        public DateFilter Date { get; set; }
        public IdFilter StatusId { get; set; }
        public List<ReservationFilter> OrFilter { get; set; }
        public ReservationOrder OrderBy {get; set;}
        public ReservationSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ReservationOrder
    {
        Id = 0,
        Table = 1,
        Date = 2,
        Status = 3,
    }

    [Flags]
    public enum ReservationSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Table = E._1,
        Date = E._2,
        Status = E._3,
    }
}
