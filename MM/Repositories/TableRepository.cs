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
    public interface ITableRepository
    {
        Task<int> Count(TableFilter TableFilter);
        Task<List<Table>> List(TableFilter TableFilter);
        Task<Table> Get(long Id);
        Task<bool> Create(Table Table);
        Task<bool> Update(Table Table);
        Task<bool> Delete(Table Table);
        Task<bool> BulkMerge(List<Table> Tables);
        Task<bool> BulkDelete(List<Table> Tables);
    }
    public class TableRepository : ITableRepository
    {
        private DataContext DataContext;
        public TableRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<TableDAO> DynamicFilter(IQueryable<TableDAO> query, TableFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.OrderId != null)
                query = query.Where(q => q.OrderId, filter.OrderId);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<TableDAO> OrFilter(IQueryable<TableDAO> query, TableFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<TableDAO> initQuery = query.Where(q => false);
            foreach (TableFilter TableFilter in filter.OrFilter)
            {
                IQueryable<TableDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Code != null)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (filter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                if (filter.OrderId != null)
                    queryable = queryable.Where(q => q.OrderId, filter.OrderId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<TableDAO> DynamicOrder(IQueryable<TableDAO> query, TableFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case TableOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case TableOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case TableOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case TableOrder.Order:
                            query = query.OrderBy(q => q.OrderId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case TableOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case TableOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case TableOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case TableOrder.Order:
                            query = query.OrderByDescending(q => q.OrderId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Table>> DynamicSelect(IQueryable<TableDAO> query, TableFilter filter)
        {
            List<Table> Tables = await query.Select(q => new Table()
            {
                Id = filter.Selects.Contains(TableSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(TableSelect.Code) ? q.Code : default(string),
                StatusId = filter.Selects.Contains(TableSelect.Status) ? q.StatusId : default(long),
                OrderId = filter.Selects.Contains(TableSelect.Order) ? q.OrderId : default(long?),
                Order = filter.Selects.Contains(TableSelect.Order) && q.Order != null ? new Order
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
            return Tables;
        }

        public async Task<int> Count(TableFilter filter)
        {
            IQueryable<TableDAO> Tables = DataContext.Table;
            Tables = DynamicFilter(Tables, filter);
            return await Tables.CountAsync();
        }

        public async Task<List<Table>> List(TableFilter filter)
        {
            if (filter == null) return new List<Table>();
            IQueryable<TableDAO> TableDAOs = DataContext.Table;
            TableDAOs = DynamicFilter(TableDAOs, filter);
            TableDAOs = DynamicOrder(TableDAOs, filter);
            List<Table> Tables = await DynamicSelect(TableDAOs, filter);
            return Tables;
        }

        public async Task<Table> Get(long Id)
        {
            Table Table = await DataContext.Table.Where(x => x.Id == Id).Select(x => new Table()
            {
                Id = x.Id,
                Code = x.Code,
                StatusId = x.StatusId,
                OrderId = x.OrderId,
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

            if (Table == null)
                return null;

            return Table;
        }
        public async Task<bool> Create(Table Table)
        {
            TableDAO TableDAO = new TableDAO();
            TableDAO.Id = Table.Id;
            TableDAO.Code = Table.Code;
            TableDAO.StatusId = Table.StatusId;
            TableDAO.OrderId = Table.OrderId;
            DataContext.Table.Add(TableDAO);
            await DataContext.SaveChangesAsync();
            Table.Id = TableDAO.Id;
            await SaveReference(Table);
            return true;
        }

        public async Task<bool> Update(Table Table)
        {
            TableDAO TableDAO = DataContext.Table.Where(x => x.Id == Table.Id).FirstOrDefault();
            if (TableDAO == null)
                return false;
            TableDAO.Id = Table.Id;
            TableDAO.Code = Table.Code;
            TableDAO.StatusId = Table.StatusId;
            TableDAO.OrderId = Table.OrderId;
            await DataContext.SaveChangesAsync();
            await SaveReference(Table);
            return true;
        }

        public async Task<bool> Delete(Table Table)
        {
            await DataContext.Table.Where(x => x.Id == Table.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Table> Tables)
        {
            List<TableDAO> TableDAOs = new List<TableDAO>();
            foreach (Table Table in Tables)
            {
                TableDAO TableDAO = new TableDAO();
                TableDAO.Id = Table.Id;
                TableDAO.Code = Table.Code;
                TableDAO.StatusId = Table.StatusId;
                TableDAO.OrderId = Table.OrderId;
                TableDAOs.Add(TableDAO);
            }
            await DataContext.BulkMergeAsync(TableDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Table> Tables)
        {
            List<long> Ids = Tables.Select(x => x.Id).ToList();
            await DataContext.Table
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(Table Table)
        {
        }
        
    }
}
