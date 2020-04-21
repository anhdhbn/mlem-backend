using Common;
using MM.Entities;
using MM.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpers;

namespace MM.Repositories
{
    public interface IReservationRepository
    {
        Task<int> Count(ReservationFilter ReservationFilter);
        Task<List<Reservation>> List(ReservationFilter ReservationFilter);
        Task<Reservation> Get(long Id);
        Task<bool> Create(Reservation Reservation);
        Task<bool> Update(Reservation Reservation);
        Task<bool> Delete(Reservation Reservation);
    }
    public class ReservationRepository : IReservationRepository
    {
        private DataContext DataContext;
        public ReservationRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ReservationDAO> DynamicFilter(IQueryable<ReservationDAO> query, ReservationFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.TableId != null)
                query = query.Where(q => q.TableId, filter.TableId);
            if (filter.Date != null)
                query = query.Where(q => q.Date, filter.Date);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            return query;
        }

        private IQueryable<ReservationDAO> DynamicOrder(IQueryable<ReservationDAO> query, ReservationFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ReservationOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ReservationOrder.Table:
                            query = query.OrderBy(q => q.TableId);
                            break;
                        case ReservationOrder.Date:
                            query = query.OrderBy(q => q.Date);
                            break;
                        case ReservationOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ReservationOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ReservationOrder.Table:
                            query = query.OrderByDescending(q => q.TableId);
                            break;
                        case ReservationOrder.Date:
                            query = query.OrderByDescending(q => q.Date);
                            break;
                        case ReservationOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Reservation>> DynamicSelect(IQueryable<ReservationDAO> query, ReservationFilter filter)
        {
            List<Reservation> Reservations = await query.Select(q => new Reservation()
            {
                Id = filter.Selects.Contains(ReservationSelect.Id) ? q.Id : default(long),
                TableId = filter.Selects.Contains(ReservationSelect.Table) ? q.TableId : default(long),
                Date = filter.Selects.Contains(ReservationSelect.Date) ? q.Date : default(DateTime),
                StatusId = filter.Selects.Contains(ReservationSelect.Status) ? q.StatusId : default(long),
                Table = filter.Selects.Contains(ReservationSelect.Table) && q.Table != null ? new Table
                {
                    Id = q.Table.Id,
                    Code = q.Table.Code,
                } : null,
            }).AsNoTracking().ToListAsync();
            return Reservations;
        }

        public async Task<int> Count(ReservationFilter filter)
        {
            IQueryable<ReservationDAO> Reservations = DataContext.Reservation;
            Reservations = DynamicFilter(Reservations, filter);
            return await Reservations.CountAsync();
        }

        public async Task<List<Reservation>> List(ReservationFilter filter)
        {
            if (filter == null) return new List<Reservation>();
            IQueryable<ReservationDAO> ReservationDAOs = DataContext.Reservation;
            ReservationDAOs = DynamicFilter(ReservationDAOs, filter);
            ReservationDAOs = DynamicOrder(ReservationDAOs, filter);
            List<Reservation> Reservations = await DynamicSelect(ReservationDAOs, filter);
            return Reservations;
        }

        public async Task<Reservation> Get(long Id)
        {
            Reservation Reservation = await DataContext.Reservation.Where(x => x.Id == Id).Select(x => new Reservation()
            {
                Id = x.Id,
                TableId = x.TableId,
                Date = x.Date,
                StatusId = x.StatusId,
                Table = x.Table == null ? null : new Table
                {
                    Id = x.Table.Id,
                    Code = x.Table.Code,
                },
            }).AsNoTracking().FirstOrDefaultAsync();

            if (Reservation == null)
                return null;

            return Reservation;
        }
        public async Task<bool> Create(Reservation Reservation)
        {
            ReservationDAO ReservationDAO = new ReservationDAO();
            ReservationDAO.Id = Reservation.Id;
            ReservationDAO.TableId = Reservation.TableId;
            ReservationDAO.Date = Reservation.Date;
            ReservationDAO.StatusId = Reservation.StatusId;
            DataContext.Reservation.Add(ReservationDAO);
            await DataContext.SaveChangesAsync();
            Reservation.Id = ReservationDAO.Id;
            return true;
        }

        public async Task<bool> Update(Reservation Reservation)
        {
            ReservationDAO ReservationDAO = DataContext.Reservation.Where(x => x.Id == Reservation.Id).FirstOrDefault();
            if (ReservationDAO == null)
                return false;
            ReservationDAO.Id = Reservation.Id;
            ReservationDAO.TableId = Reservation.TableId;
            ReservationDAO.Date = Reservation.Date;
            ReservationDAO.StatusId = Reservation.StatusId;
            await DataContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(Reservation Reservation)
        {
            await DataContext.Reservation.Where(x => x.Id == Reservation.Id).DeleteFromQueryAsync();
            return true;
        }
    }
}
