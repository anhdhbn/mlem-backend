using System;
using System.Collections.Generic;

namespace MM.Models
{
    public partial class OrderDAO
    {
        public OrderDAO()
        {
            OrderContents = new HashSet<OrderContentDAO>();
            Tables = new HashSet<TableDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? PayDate { get; set; }
        public long AccountId { get; set; }
        public long NumOfTable { get; set; }
        public long NumOfPerson { get; set; }
        public string Descreption { get; set; }
        public long StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual AccountDAO Account { get; set; }
        public virtual ICollection<OrderContentDAO> OrderContents { get; set; }
        public virtual ICollection<TableDAO> Tables { get; set; }
    }
}
