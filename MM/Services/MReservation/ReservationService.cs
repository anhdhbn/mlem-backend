using Common;
using Helpers;
using MM.Entities;
using MM.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MM.Services.MReservation
{
    public interface IReservationService :  IServiceScoped
    {
        Task<int> Count(ReservationFilter ReservationFilter);
        Task<List<Reservation>> List(ReservationFilter ReservationFilter);
        Task<Reservation> Get(long Id);
        Task<Reservation> Create(Reservation Reservation);
        Task<Reservation> Update(Reservation Reservation);
        Task<Reservation> Delete(Reservation Reservation);
    }

    public class ReservationService : IReservationService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IReservationValidator ReservationValidator;

        public ReservationService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IReservationValidator ReservationValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ReservationValidator = ReservationValidator;
        }
        public async Task<int> Count(ReservationFilter ReservationFilter)
        {
            try
            {
                int result = await UOW.ReservationRepository.Count(ReservationFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ReservationService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Reservation>> List(ReservationFilter ReservationFilter)
        {
            try
            {
                List<Reservation> Reservations = await UOW.ReservationRepository.List(ReservationFilter);
                return Reservations;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ReservationService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Reservation> Get(long Id)
        {
            Reservation Reservation = await UOW.ReservationRepository.Get(Id);
            if (Reservation == null)
                return null;
            return Reservation;
        }
       
        public async Task<Reservation> Create(Reservation Reservation)
        {
            if (!await ReservationValidator.Create(Reservation))
                return Reservation;

            try
            {
                await UOW.Begin();
                await UOW.ReservationRepository.Create(Reservation);
                await UOW.Commit();

                await Logging.CreateAuditLog(Reservation, new { }, nameof(ReservationService));
                return await UOW.ReservationRepository.Get(Reservation.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ReservationService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Reservation> Update(Reservation Reservation)
        {
            if (!await ReservationValidator.Update(Reservation))
                return Reservation;
            try
            {
                var oldData = await UOW.ReservationRepository.Get(Reservation.Id);

                await UOW.Begin();
                await UOW.ReservationRepository.Update(Reservation);
                await UOW.Commit();

                var newData = await UOW.ReservationRepository.Get(Reservation.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(ReservationService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ReservationService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Reservation> Delete(Reservation Reservation)
        {
            if (!await ReservationValidator.Delete(Reservation))
                return Reservation;

            try
            {
                await UOW.Begin();
                await UOW.ReservationRepository.Delete(Reservation);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Reservation, nameof(ReservationService));
                return Reservation;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ReservationService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
    }
}
