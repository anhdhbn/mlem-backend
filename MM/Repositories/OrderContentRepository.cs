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
    public interface IOrderContentRepository
    {
        Task<int> Count(OrderContentFilter OrderContentFilter);
        Task<List<OrderContent>> List(OrderContentFilter OrderContentFilter);
        Task<OrderContent> Get(long Id);
        Task<bool> Create(OrderContent OrderContent);
        Task<bool> Update(OrderContent OrderContent);
        Task<bool> Delete(OrderContent OrderContent);
        Task<bool> BulkMerge(List<OrderContent> OrderContents);
        Task<bool> BulkDelete(List<OrderContent> OrderContents);
    }
    public class OrderContentRepository : IOrderContentRepository
    {
        private DataContext DataContext;
        public OrderContentRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<OrderContentDAO> DynamicFilter(IQueryable<OrderContentDAO> query, OrderContentFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.OrderId != null)
                query = query.Where(q => q.OrderId, filter.OrderId);
            if (filter.FoodFoodTypeMappingId != null)
                query = query.Where(q => q.FoodFoodTypeMappingId, filter.FoodFoodTypeMappingId);
            if (filter.Quantity != null)
                query = query.Where(q => q.Quantity, filter.Quantity);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<OrderContentDAO> OrFilter(IQueryable<OrderContentDAO> query, OrderContentFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<OrderContentDAO> initQuery = query.Where(q => false);
            foreach (OrderContentFilter OrderContentFilter in filter.OrFilter)
            {
                IQueryable<OrderContentDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Code != null)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (filter.OrderId != null)
                    queryable = queryable.Where(q => q.OrderId, filter.OrderId);
                if (filter.FoodFoodTypeMappingId != null)
                    queryable = queryable.Where(q => q.FoodFoodTypeMappingId, filter.FoodFoodTypeMappingId);
                if (filter.Quantity != null)
                    queryable = queryable.Where(q => q.Quantity, filter.Quantity);
                if (filter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<OrderContentDAO> DynamicOrder(IQueryable<OrderContentDAO> query, OrderContentFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case OrderContentOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case OrderContentOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case OrderContentOrder.Order:
                            query = query.OrderBy(q => q.OrderId);
                            break;
                        case OrderContentOrder.FoodFoodTypeMapping:
                            query = query.OrderBy(q => q.FoodFoodTypeMappingId);
                            break;
                        case OrderContentOrder.Quantity:
                            query = query.OrderBy(q => q.Quantity);
                            break;
                        case OrderContentOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case OrderContentOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case OrderContentOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case OrderContentOrder.Order:
                            query = query.OrderByDescending(q => q.OrderId);
                            break;
                        case OrderContentOrder.FoodFoodTypeMapping:
                            query = query.OrderByDescending(q => q.FoodFoodTypeMappingId);
                            break;
                        case OrderContentOrder.Quantity:
                            query = query.OrderByDescending(q => q.Quantity);
                            break;
                        case OrderContentOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<OrderContent>> DynamicSelect(IQueryable<OrderContentDAO> query, OrderContentFilter filter)
        {
            List<OrderContent> OrderContents = await query.Select(q => new OrderContent()
            {
                Id = filter.Selects.Contains(OrderContentSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(OrderContentSelect.Code) ? q.Code : default(string),
                OrderId = filter.Selects.Contains(OrderContentSelect.Order) ? q.OrderId : default(long),
                FoodFoodTypeMappingId = filter.Selects.Contains(OrderContentSelect.FoodFoodTypeMapping) ? q.FoodFoodTypeMappingId : default(long),
                Quantity = filter.Selects.Contains(OrderContentSelect.Quantity) ? q.Quantity : default(long),
                StatusId = filter.Selects.Contains(OrderContentSelect.Status) ? q.StatusId : default(long),
                FoodFoodTypeMapping = filter.Selects.Contains(OrderContentSelect.FoodFoodTypeMapping) && q.FoodFoodTypeMapping != null ? new FoodFoodTypeMapping
                {
                    Id = q.FoodFoodTypeMapping.Id,
                    FoodId = q.FoodFoodTypeMapping.FoodId,
                    FoodTypeId = q.FoodFoodTypeMapping.FoodTypeId,
                } : null,
                Order = filter.Selects.Contains(OrderContentSelect.Order) && q.Order != null ? new Order
                {
                    Id = q.Order.Id,
                    Code = q.Order.Code,
                    OrderDate = q.Order.OrderDate,
                    PayDate = q.Order.PayDate,
                    AccountId = q.Order.AccountId,
                    NumOfTable = q.Order.NumOfTable,
                    NumOfPerson = q.Order.NumOfPerson,
                    Descreption = q.Order.Descreption,
                    StatusId = q.Order.StatusId,
                } : null,
            }).AsNoTracking().ToListAsync();
            return OrderContents;
        }

        public async Task<int> Count(OrderContentFilter filter)
        {
            IQueryable<OrderContentDAO> OrderContents = DataContext.OrderContent;
            OrderContents = DynamicFilter(OrderContents, filter);
            return await OrderContents.CountAsync();
        }

        public async Task<List<OrderContent>> List(OrderContentFilter filter)
        {
            if (filter == null) return new List<OrderContent>();
            IQueryable<OrderContentDAO> OrderContentDAOs = DataContext.OrderContent;
            OrderContentDAOs = DynamicFilter(OrderContentDAOs, filter);
            OrderContentDAOs = DynamicOrder(OrderContentDAOs, filter);
            List<OrderContent> OrderContents = await DynamicSelect(OrderContentDAOs, filter);
            return OrderContents;
        }

        public async Task<OrderContent> Get(long Id)
        {
            OrderContent OrderContent = await DataContext.OrderContent.Where(x => x.Id == Id).Select(x => new OrderContent()
            {
                Id = x.Id,
                Code = x.Code,
                OrderId = x.OrderId,
                FoodFoodTypeMappingId = x.FoodFoodTypeMappingId,
                Quantity = x.Quantity,
                StatusId = x.StatusId,
                FoodFoodTypeMapping = x.FoodFoodTypeMapping == null ? null : new FoodFoodTypeMapping
                {
                    Id = x.FoodFoodTypeMapping.Id,
                    FoodId = x.FoodFoodTypeMapping.FoodId,
                    FoodTypeId = x.FoodFoodTypeMapping.FoodTypeId,
                },
                Order = x.Order == null ? null : new Order
                {
                    Id = x.Order.Id,
                    Code = x.Order.Code,
                    OrderDate = x.Order.OrderDate,
                    PayDate = x.Order.PayDate,
                    AccountId = x.Order.AccountId,
                    NumOfTable = x.Order.NumOfTable,
                    NumOfPerson = x.Order.NumOfPerson,
                    Descreption = x.Order.Descreption,
                    StatusId = x.Order.StatusId,
                },
            }).AsNoTracking().FirstOrDefaultAsync();

            if (OrderContent == null)
                return null;

            return OrderContent;
        }
        public async Task<bool> Create(OrderContent OrderContent)
        {
            OrderContentDAO OrderContentDAO = new OrderContentDAO();
            OrderContentDAO.Id = OrderContent.Id;
            OrderContentDAO.Code = OrderContent.Code;
            OrderContentDAO.OrderId = OrderContent.OrderId;
            OrderContentDAO.FoodFoodTypeMappingId = OrderContent.FoodFoodTypeMappingId;
            OrderContentDAO.Quantity = OrderContent.Quantity;
            OrderContentDAO.StatusId = OrderContent.StatusId;
            DataContext.OrderContent.Add(OrderContentDAO);
            await DataContext.SaveChangesAsync();
            OrderContent.Id = OrderContentDAO.Id;
            await SaveReference(OrderContent);
            return true;
        }

        public async Task<bool> Update(OrderContent OrderContent)
        {
            OrderContentDAO OrderContentDAO = DataContext.OrderContent.Where(x => x.Id == OrderContent.Id).FirstOrDefault();
            if (OrderContentDAO == null)
                return false;
            OrderContentDAO.Id = OrderContent.Id;
            OrderContentDAO.Code = OrderContent.Code;
            OrderContentDAO.OrderId = OrderContent.OrderId;
            OrderContentDAO.FoodFoodTypeMappingId = OrderContent.FoodFoodTypeMappingId;
            OrderContentDAO.Quantity = OrderContent.Quantity;
            OrderContentDAO.StatusId = OrderContent.StatusId;
            await DataContext.SaveChangesAsync();
            await SaveReference(OrderContent);
            return true;
        }

        public async Task<bool> Delete(OrderContent OrderContent)
        {
            await DataContext.OrderContent.Where(x => x.Id == OrderContent.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<OrderContent> OrderContents)
        {
            List<OrderContentDAO> OrderContentDAOs = new List<OrderContentDAO>();
            foreach (OrderContent OrderContent in OrderContents)
            {
                OrderContentDAO OrderContentDAO = new OrderContentDAO();
                OrderContentDAO.Id = OrderContent.Id;
                OrderContentDAO.Code = OrderContent.Code;
                OrderContentDAO.OrderId = OrderContent.OrderId;
                OrderContentDAO.FoodFoodTypeMappingId = OrderContent.FoodFoodTypeMappingId;
                OrderContentDAO.Quantity = OrderContent.Quantity;
                OrderContentDAO.StatusId = OrderContent.StatusId;
                OrderContentDAOs.Add(OrderContentDAO);
            }
            await DataContext.BulkMergeAsync(OrderContentDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<OrderContent> OrderContents)
        {
            List<long> Ids = OrderContents.Select(x => x.Id).ToList();
            await DataContext.OrderContent
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(OrderContent OrderContent)
        {
        }
        
    }
}
