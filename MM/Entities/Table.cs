using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MM.Entities
{
    public class Table : DataEntity,  IEquatable<Table>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long? OrderId { get; set; }
        public Order Order { get; set; }

        public bool Equals(Table other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class TableFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public IdFilter OrderId { get; set; }
        public List<TableFilter> OrFilter { get; set; }
        public TableOrder OrderBy {get; set;}
        public TableSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TableOrder
    {
        Id = 0,
        Code = 1,
        Order = 2,
    }

    [Flags]
    public enum TableSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Order = E._2,
    }
}
