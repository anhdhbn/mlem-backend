using System;
using System.Collections.Generic;

namespace MM.Models
{
    public partial class ReservationDAO
    {
        public long Id { get; set; }
        public long TableId { get; set; }
        public DateTime Date { get; set; }
        public long StatusId { get; set; }

        public virtual TableDAO Table { get; set; }
    }
}
