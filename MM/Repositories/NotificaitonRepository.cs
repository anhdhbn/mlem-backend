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
    public interface INotificaitonRepository
    {
        Task<int> Count(NotificaitonFilter NotificaitonFilter);
        Task<List<Notificaiton>> List(NotificaitonFilter NotificaitonFilter);
        Task<Notificaiton> Get(long Id);
        Task<bool> Create(Notificaiton Notificaiton);
        Task<bool> Update(Notificaiton Notificaiton);
        Task<bool> Delete(Notificaiton Notificaiton);
        Task<bool> BulkMerge(List<Notificaiton> Notificaitons);
        Task<bool> BulkDelete(List<Notificaiton> Notificaitons);
    }
    public class NotificaitonRepository : INotificaitonRepository
    {
        private DataContext DataContext;
        public NotificaitonRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<NotificaitonDAO> DynamicFilter(IQueryable<NotificaitonDAO> query, NotificaitonFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.AccountId != null)
                query = query.Where(q => q.AccountId, filter.AccountId);
            if (filter.Content != null)
                query = query.Where(q => q.Content, filter.Content);
            if (filter.Time != null)
                query = query.Where(q => q.Time, filter.Time);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<NotificaitonDAO> OrFilter(IQueryable<NotificaitonDAO> query, NotificaitonFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<NotificaitonDAO> initQuery = query.Where(q => false);
            foreach (NotificaitonFilter NotificaitonFilter in filter.OrFilter)
            {
                IQueryable<NotificaitonDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.AccountId != null)
                    queryable = queryable.Where(q => q.AccountId, filter.AccountId);
                if (filter.Content != null)
                    queryable = queryable.Where(q => q.Content, filter.Content);
                if (filter.Time != null)
                    queryable = queryable.Where(q => q.Time, filter.Time);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<NotificaitonDAO> DynamicOrder(IQueryable<NotificaitonDAO> query, NotificaitonFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case NotificaitonOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case NotificaitonOrder.Account:
                            query = query.OrderBy(q => q.AccountId);
                            break;
                        case NotificaitonOrder.Content:
                            query = query.OrderBy(q => q.Content);
                            break;
                        case NotificaitonOrder.Time:
                            query = query.OrderBy(q => q.Time);
                            break;
                        case NotificaitonOrder.Unread:
                            query = query.OrderBy(q => q.Unread);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case NotificaitonOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case NotificaitonOrder.Account:
                            query = query.OrderByDescending(q => q.AccountId);
                            break;
                        case NotificaitonOrder.Content:
                            query = query.OrderByDescending(q => q.Content);
                            break;
                        case NotificaitonOrder.Time:
                            query = query.OrderByDescending(q => q.Time);
                            break;
                        case NotificaitonOrder.Unread:
                            query = query.OrderByDescending(q => q.Unread);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Notificaiton>> DynamicSelect(IQueryable<NotificaitonDAO> query, NotificaitonFilter filter)
        {
            List<Notificaiton> Notificaitons = await query.Select(q => new Notificaiton()
            {
                Id = filter.Selects.Contains(NotificaitonSelect.Id) ? q.Id : default(long),
                AccountId = filter.Selects.Contains(NotificaitonSelect.Account) ? q.AccountId : default(long),
                Content = filter.Selects.Contains(NotificaitonSelect.Content) ? q.Content : default(string),
                Time = filter.Selects.Contains(NotificaitonSelect.Time) ? q.Time : default(DateTime),
                Unread = filter.Selects.Contains(NotificaitonSelect.Unread) ? q.Unread : default(bool),
                Account = filter.Selects.Contains(NotificaitonSelect.Account) && q.Account != null ? new Account
                {
                    Id = q.Account.Id,
                    DisplayName = q.Account.DisplayName,
                    Email = q.Account.Email,
                    Phone = q.Account.Phone,
                    Password = q.Account.Password,
                    Salt = q.Account.Salt,
                    PasswordRecoveryCode = q.Account.PasswordRecoveryCode,
                    ExpiredTimeCode = q.Account.ExpiredTimeCode,
                    Address = q.Account.Address,
                    Dob = q.Account.Dob,
                    Avatar = q.Account.Avatar,
                    RoleId = q.Account.RoleId,
                } : null,
            }).AsNoTracking().ToListAsync();
            return Notificaitons;
        }

        public async Task<int> Count(NotificaitonFilter filter)
        {
            IQueryable<NotificaitonDAO> Notificaitons = DataContext.Notificaiton;
            Notificaitons = DynamicFilter(Notificaitons, filter);
            return await Notificaitons.CountAsync();
        }

        public async Task<List<Notificaiton>> List(NotificaitonFilter filter)
        {
            if (filter == null) return new List<Notificaiton>();
            IQueryable<NotificaitonDAO> NotificaitonDAOs = DataContext.Notificaiton;
            NotificaitonDAOs = DynamicFilter(NotificaitonDAOs, filter);
            NotificaitonDAOs = DynamicOrder(NotificaitonDAOs, filter);
            List<Notificaiton> Notificaitons = await DynamicSelect(NotificaitonDAOs, filter);
            return Notificaitons;
        }

        public async Task<Notificaiton> Get(long Id)
        {
            Notificaiton Notificaiton = await DataContext.Notificaiton.Where(x => x.Id == Id).Select(x => new Notificaiton()
            {
                Id = x.Id,
                AccountId = x.AccountId,
                Content = x.Content,
                Time = x.Time,
                Unread = x.Unread,
                Account = x.Account == null ? null : new Account
                {
                    Id = x.Account.Id,
                    DisplayName = x.Account.DisplayName,
                    Email = x.Account.Email,
                    Phone = x.Account.Phone,
                    Password = x.Account.Password,
                    Salt = x.Account.Salt,
                    PasswordRecoveryCode = x.Account.PasswordRecoveryCode,
                    ExpiredTimeCode = x.Account.ExpiredTimeCode,
                    Address = x.Account.Address,
                    Dob = x.Account.Dob,
                    Avatar = x.Account.Avatar,
                    RoleId = x.Account.RoleId,
                },
            }).AsNoTracking().FirstOrDefaultAsync();

            if (Notificaiton == null)
                return null;

            return Notificaiton;
        }
        public async Task<bool> Create(Notificaiton Notificaiton)
        {
            NotificaitonDAO NotificaitonDAO = new NotificaitonDAO();
            NotificaitonDAO.Id = Notificaiton.Id;
            NotificaitonDAO.AccountId = Notificaiton.AccountId;
            NotificaitonDAO.Content = Notificaiton.Content;
            NotificaitonDAO.Time = Notificaiton.Time;
            NotificaitonDAO.Unread = Notificaiton.Unread;
            DataContext.Notificaiton.Add(NotificaitonDAO);
            await DataContext.SaveChangesAsync();
            Notificaiton.Id = NotificaitonDAO.Id;
            await SaveReference(Notificaiton);
            return true;
        }

        public async Task<bool> Update(Notificaiton Notificaiton)
        {
            NotificaitonDAO NotificaitonDAO = DataContext.Notificaiton.Where(x => x.Id == Notificaiton.Id).FirstOrDefault();
            if (NotificaitonDAO == null)
                return false;
            NotificaitonDAO.Id = Notificaiton.Id;
            NotificaitonDAO.AccountId = Notificaiton.AccountId;
            NotificaitonDAO.Content = Notificaiton.Content;
            NotificaitonDAO.Time = Notificaiton.Time;
            NotificaitonDAO.Unread = Notificaiton.Unread;
            await DataContext.SaveChangesAsync();
            await SaveReference(Notificaiton);
            return true;
        }

        public async Task<bool> Delete(Notificaiton Notificaiton)
        {
            await DataContext.Notificaiton.Where(x => x.Id == Notificaiton.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Notificaiton> Notificaitons)
        {
            List<NotificaitonDAO> NotificaitonDAOs = new List<NotificaitonDAO>();
            foreach (Notificaiton Notificaiton in Notificaitons)
            {
                NotificaitonDAO NotificaitonDAO = new NotificaitonDAO();
                NotificaitonDAO.Id = Notificaiton.Id;
                NotificaitonDAO.AccountId = Notificaiton.AccountId;
                NotificaitonDAO.Content = Notificaiton.Content;
                NotificaitonDAO.Time = Notificaiton.Time;
                NotificaitonDAO.Unread = Notificaiton.Unread;
                NotificaitonDAOs.Add(NotificaitonDAO);
            }
            await DataContext.BulkMergeAsync(NotificaitonDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Notificaiton> Notificaitons)
        {
            List<long> Ids = Notificaitons.Select(x => x.Id).ToList();
            await DataContext.Notificaiton
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(Notificaiton Notificaiton)
        {
        }
        
    }
}
