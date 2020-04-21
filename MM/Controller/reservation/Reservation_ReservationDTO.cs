using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using MM.Entities;

namespace MM.Controller.reservation
{
    public class Reservation_ReservationDTO : DataDTO
    {
        public long Id { get; set; }
        public long TableId { get; set; }
        public DateTime Date { get; set; }
        public long StatusId { get; set; }
        public Reservation_TableDTO Table { get; set; }
        public Reservation_ReservationDTO() {}
        public Reservation_ReservationDTO(Reservation Reservation)
        {
            this.Id = Reservation.Id;
            this.TableId = Reservation.TableId;
            this.Date = Reservation.Date;
            this.StatusId = Reservation.StatusId;
            this.Table = Reservation.Table == null ? null : new Reservation_TableDTO(Reservation.Table);
            this.Errors = Reservation.Errors;
        }
    }

    public class Reservation_ReservationFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter TableId { get; set; }
        public DateFilter Date { get; set; }
        public IdFilter StatusId { get; set; }
        public ReservationOrder OrderBy { get; set; }
    }
}
