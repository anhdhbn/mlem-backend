using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using MM.Entities;
using MM;
using MM.Repositories;

namespace MM.Services.MReservation
{
    public interface IReservationValidator : IServiceScoped
    {
        Task<bool> Create(Reservation Reservation);
        Task<bool> Update(Reservation Reservation);
        Task<bool> Delete(Reservation Reservation);
        Task<bool> BulkDelete(List<Reservation> Reservations);
        Task<bool> Import(List<Reservation> Reservations);
    }

    public class ReservationValidator : IReservationValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ReservationValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Reservation Reservation)
        {
            ReservationFilter ReservationFilter = new ReservationFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Reservation.Id },
                Selects = ReservationSelect.Id
            };

            int count = await UOW.ReservationRepository.Count(ReservationFilter);
            if (count == 0)
                Reservation.AddError(nameof(ReservationValidator), nameof(Reservation.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(Reservation Reservation)
        {
            return Reservation.IsValidated;
        }

        public async Task<bool> Update(Reservation Reservation)
        {
            if (await ValidateId(Reservation))
            {
            }
            return Reservation.IsValidated;
        }

        public async Task<bool> Delete(Reservation Reservation)
        {
            if (await ValidateId(Reservation))
            {
            }
            return Reservation.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Reservation> Reservations)
        {
            return true;
        }
        
        public async Task<bool> Import(List<Reservation> Reservations)
        {
            return true;
        }
    }
}
