using System;
using System.Collections.Generic;

namespace MM.Models
{
    public partial class TableDAO
    {
        public TableDAO()
        {
            Reservations = new HashSet<ReservationDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public long? OrderId { get; set; }

        public virtual OrderDAO Order { get; set; }
        public virtual ICollection<ReservationDAO> Reservations { get; set; }
    }
}
