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
    public interface IOrderRepository
    {
        Task<int> Count(OrderFilter OrderFilter);
        Task<List<Order>> List(OrderFilter OrderFilter);
        Task<Order> Get(long Id);
        Task<bool> Create(Order Order);
        Task<bool> Update(Order Order);
        Task<bool> Delete(Order Order);
        Task<bool> BulkMerge(List<Order> Orders);
        Task<bool> BulkDelete(List<Order> Orders);
    }
    public class OrderRepository : IOrderRepository
    {
        private DataContext DataContext;
        public OrderRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<OrderDAO> DynamicFilter(IQueryable<OrderDAO> query, OrderFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.OrderDate != null)
                query = query.Where(q => q.OrderDate, filter.OrderDate);
            if (filter.PayDate != null)
                query = query.Where(q => q.PayDate, filter.PayDate);
            if (filter.AccountId != null)
                query = query.Where(q => q.AccountId, filter.AccountId);
            if (filter.NumOfTable != null)
                query = query.Where(q => q.NumOfTable, filter.NumOfTable);
            if (filter.NumOfPerson != null)
                query = query.Where(q => q.NumOfPerson, filter.NumOfPerson);
            if (filter.Descreption != null)
                query = query.Where(q => q.Descreption, filter.Descreption);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<OrderDAO> OrFilter(IQueryable<OrderDAO> query, OrderFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<OrderDAO> initQuery = query.Where(q => false);
            foreach (OrderFilter OrderFilter in filter.OrFilter)
            {
                IQueryable<OrderDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Code != null)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (filter.OrderDate != null)
                    queryable = queryable.Where(q => q.OrderDate, filter.OrderDate);
                if (filter.PayDate != null)
                    queryable = queryable.Where(q => q.PayDate, filter.PayDate);
                if (filter.AccountId != null)
                    queryable = queryable.Where(q => q.AccountId, filter.AccountId);
                if (filter.NumOfTable != null)
                    queryable = queryable.Where(q => q.NumOfTable, filter.NumOfTable);
                if (filter.NumOfPerson != null)
                    queryable = queryable.Where(q => q.NumOfPerson, filter.NumOfPerson);
                if (filter.Descreption != null)
                    queryable = queryable.Where(q => q.Descreption, filter.Descreption);
                if (filter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<OrderDAO> DynamicOrder(IQueryable<OrderDAO> query, OrderFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case OrderOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case OrderOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case OrderOrder.OrderDate:
                            query = query.OrderBy(q => q.OrderDate);
                            break;
                        case OrderOrder.PayDate:
                            query = query.OrderBy(q => q.PayDate);
                            break;
                        case OrderOrder.Account:
                            query = query.OrderBy(q => q.AccountId);
                            break;
                        case OrderOrder.NumOfTable:
                            query = query.OrderBy(q => q.NumOfTable);
                            break;
                        case OrderOrder.NumOfPerson:
                            query = query.OrderBy(q => q.NumOfPerson);
                            break;
                        case OrderOrder.Descreption:
                            query = query.OrderBy(q => q.Descreption);
                            break;
                        case OrderOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case OrderOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case OrderOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case OrderOrder.OrderDate:
                            query = query.OrderByDescending(q => q.OrderDate);
                            break;
                        case OrderOrder.PayDate:
                            query = query.OrderByDescending(q => q.PayDate);
                            break;
                        case OrderOrder.Account:
                            query = query.OrderByDescending(q => q.AccountId);
                            break;
                        case OrderOrder.NumOfTable:
                            query = query.OrderByDescending(q => q.NumOfTable);
                            break;
                        case OrderOrder.NumOfPerson:
                            query = query.OrderByDescending(q => q.NumOfPerson);
                            break;
                        case OrderOrder.Descreption:
                            query = query.OrderByDescending(q => q.Descreption);
                            break;
                        case OrderOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Order>> DynamicSelect(IQueryable<OrderDAO> query, OrderFilter filter)
        {
            List<Order> Orders = await query.Select(q => new Order()
            {
                Id = filter.Selects.Contains(OrderSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(OrderSelect.Code) ? q.Code : default(string),
                OrderDate = filter.Selects.Contains(OrderSelect.OrderDate) ? q.OrderDate : default(DateTime),
                PayDate = filter.Selects.Contains(OrderSelect.PayDate) ? q.PayDate : default(DateTime?),
                AccountId = filter.Selects.Contains(OrderSelect.Account) ? q.AccountId : default(long),
                NumOfTable = filter.Selects.Contains(OrderSelect.NumOfTable) ? q.NumOfTable : default(long),
                NumOfPerson = filter.Selects.Contains(OrderSelect.NumOfPerson) ? q.NumOfPerson : default(long),
                Descreption = filter.Selects.Contains(OrderSelect.Descreption) ? q.Descreption : default(string),
                StatusId = filter.Selects.Contains(OrderSelect.Status) ? q.StatusId : default(long),
                Account = filter.Selects.Contains(OrderSelect.Account) && q.Account != null ? new Account
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
            return Orders;
        }

        public async Task<int> Count(OrderFilter filter)
        {
            IQueryable<OrderDAO> Orders = DataContext.Order;
            Orders = DynamicFilter(Orders, filter);
            return await Orders.CountAsync();
        }

        public async Task<List<Order>> List(OrderFilter filter)
        {
            if (filter == null) return new List<Order>();
            IQueryable<OrderDAO> OrderDAOs = DataContext.Order;
            OrderDAOs = DynamicFilter(OrderDAOs, filter);
            OrderDAOs = DynamicOrder(OrderDAOs, filter);
            List<Order> Orders = await DynamicSelect(OrderDAOs, filter);
            return Orders;
        }

        public async Task<Order> Get(long Id)
        {
            Order Order = await DataContext.Order.Where(x => x.Id == Id).Select(x => new Order()
            {
                Id = x.Id,
                Code = x.Code,
                OrderDate = x.OrderDate,
                PayDate = x.PayDate,
                AccountId = x.AccountId,
                NumOfTable = x.NumOfTable,
                NumOfPerson = x.NumOfPerson,
                Descreption = x.Descreption,
                StatusId = x.StatusId,
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

            if (Order == null)
                return null;

            return Order;
        }
        public async Task<bool> Create(Order Order)
        {
            OrderDAO OrderDAO = new OrderDAO();
            OrderDAO.Id = Order.Id;
            OrderDAO.Code = Order.Code;
            OrderDAO.OrderDate = Order.OrderDate;
            OrderDAO.PayDate = Order.PayDate;
            OrderDAO.AccountId = Order.AccountId;
            OrderDAO.NumOfTable = Order.NumOfTable;
            OrderDAO.NumOfPerson = Order.NumOfPerson;
            OrderDAO.Descreption = Order.Descreption;
            OrderDAO.StatusId = Order.StatusId;
            OrderDAO.CreatedAt = StaticParams.DateTimeNow;
            OrderDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Order.Add(OrderDAO);
            await DataContext.SaveChangesAsync();
            Order.Id = OrderDAO.Id;
            await SaveReference(Order);
            return true;
        }

        public async Task<bool> Update(Order Order)
        {
            OrderDAO OrderDAO = DataContext.Order.Where(x => x.Id == Order.Id).FirstOrDefault();
            if (OrderDAO == null)
                return false;
            OrderDAO.Id = Order.Id;
            OrderDAO.Code = Order.Code;
            OrderDAO.OrderDate = Order.OrderDate;
            OrderDAO.PayDate = Order.PayDate;
            OrderDAO.AccountId = Order.AccountId;
            OrderDAO.NumOfTable = Order.NumOfTable;
            OrderDAO.NumOfPerson = Order.NumOfPerson;
            OrderDAO.Descreption = Order.Descreption;
            OrderDAO.StatusId = Order.StatusId;
            OrderDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Order);
            return true;
        }

        public async Task<bool> Delete(Order Order)
        {
            await DataContext.Order.Where(x => x.Id == Order.Id).UpdateFromQueryAsync(x => new OrderDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Order> Orders)
        {
            List<OrderDAO> OrderDAOs = new List<OrderDAO>();
            foreach (Order Order in Orders)
            {
                OrderDAO OrderDAO = new OrderDAO();
                OrderDAO.Id = Order.Id;
                OrderDAO.Code = Order.Code;
                OrderDAO.OrderDate = Order.OrderDate;
                OrderDAO.PayDate = Order.PayDate;
                OrderDAO.AccountId = Order.AccountId;
                OrderDAO.NumOfTable = Order.NumOfTable;
                OrderDAO.NumOfPerson = Order.NumOfPerson;
                OrderDAO.Descreption = Order.Descreption;
                OrderDAO.StatusId = Order.StatusId;
                OrderDAO.CreatedAt = StaticParams.DateTimeNow;
                OrderDAO.UpdatedAt = StaticParams.DateTimeNow;
                OrderDAOs.Add(OrderDAO);
            }
            await DataContext.BulkMergeAsync(OrderDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Order> Orders)
        {
            List<long> Ids = Orders.Select(x => x.Id).ToList();
            await DataContext.Order
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new OrderDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Order Order)
        {
        }
        
    }
}
