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
    public interface IFoodGroupingRepository
    {
        Task<int> Count(FoodGroupingFilter FoodGroupingFilter);
        Task<List<FoodGrouping>> List(FoodGroupingFilter FoodGroupingFilter);
        Task<FoodGrouping> Get(long Id);
        Task<bool> Create(FoodGrouping FoodGrouping);
        Task<bool> Update(FoodGrouping FoodGrouping);
        Task<bool> Delete(FoodGrouping FoodGrouping);
        Task<bool> BulkMerge(List<FoodGrouping> FoodGroupings);
        Task<bool> BulkDelete(List<FoodGrouping> FoodGroupings);
    }
    public class FoodGroupingRepository : IFoodGroupingRepository
    {
        private DataContext DataContext;
        public FoodGroupingRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<FoodGroupingDAO> DynamicFilter(IQueryable<FoodGroupingDAO> query, FoodGroupingFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<FoodGroupingDAO> OrFilter(IQueryable<FoodGroupingDAO> query, FoodGroupingFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<FoodGroupingDAO> initQuery = query.Where(q => false);
            foreach (FoodGroupingFilter FoodGroupingFilter in filter.OrFilter)
            {
                IQueryable<FoodGroupingDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<FoodGroupingDAO> DynamicOrder(IQueryable<FoodGroupingDAO> query, FoodGroupingFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case FoodGroupingOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case FoodGroupingOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case FoodGroupingOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case FoodGroupingOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case FoodGroupingOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case FoodGroupingOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<FoodGrouping>> DynamicSelect(IQueryable<FoodGroupingDAO> query, FoodGroupingFilter filter)
        {
            List<FoodGrouping> FoodGroupings = await query.Select(q => new FoodGrouping()
            {
                Id = filter.Selects.Contains(FoodGroupingSelect.Id) ? q.Id : default(long),
                Name = filter.Selects.Contains(FoodGroupingSelect.Name) ? q.Name : default(string),
                StatusId = filter.Selects.Contains(FoodGroupingSelect.Status) ? q.StatusId : default(long),
            }).AsNoTracking().ToListAsync();
            return FoodGroupings;
        }

        public async Task<int> Count(FoodGroupingFilter filter)
        {
            IQueryable<FoodGroupingDAO> FoodGroupings = DataContext.FoodGrouping;
            FoodGroupings = DynamicFilter(FoodGroupings, filter);
            return await FoodGroupings.CountAsync();
        }

        public async Task<List<FoodGrouping>> List(FoodGroupingFilter filter)
        {
            if (filter == null) return new List<FoodGrouping>();
            IQueryable<FoodGroupingDAO> FoodGroupingDAOs = DataContext.FoodGrouping;
            FoodGroupingDAOs = DynamicFilter(FoodGroupingDAOs, filter);
            FoodGroupingDAOs = DynamicOrder(FoodGroupingDAOs, filter);
            List<FoodGrouping> FoodGroupings = await DynamicSelect(FoodGroupingDAOs, filter);
            return FoodGroupings;
        }

        public async Task<FoodGrouping> Get(long Id)
        {
            FoodGrouping FoodGrouping = await DataContext.FoodGrouping.Where(x => x.Id == Id).Select(x => new FoodGrouping()
            {
                Id = x.Id,
                Name = x.Name,
                StatusId = x.StatusId,
            }).AsNoTracking().FirstOrDefaultAsync();

            if (FoodGrouping == null)
                return null;

            return FoodGrouping;
        }
        public async Task<bool> Create(FoodGrouping FoodGrouping)
        {
            FoodGroupingDAO FoodGroupingDAO = new FoodGroupingDAO();
            FoodGroupingDAO.Id = FoodGrouping.Id;
            FoodGroupingDAO.Name = FoodGrouping.Name;
            FoodGroupingDAO.StatusId = FoodGrouping.StatusId;
            DataContext.FoodGrouping.Add(FoodGroupingDAO);
            await DataContext.SaveChangesAsync();
            FoodGrouping.Id = FoodGroupingDAO.Id;
            await SaveReference(FoodGrouping);
            return true;
        }

        public async Task<bool> Update(FoodGrouping FoodGrouping)
        {
            FoodGroupingDAO FoodGroupingDAO = DataContext.FoodGrouping.Where(x => x.Id == FoodGrouping.Id).FirstOrDefault();
            if (FoodGroupingDAO == null)
                return false;
            FoodGroupingDAO.Id = FoodGrouping.Id;
            FoodGroupingDAO.Name = FoodGrouping.Name;
            FoodGroupingDAO.StatusId = FoodGrouping.StatusId;
            await DataContext.SaveChangesAsync();
            await SaveReference(FoodGrouping);
            return true;
        }

        public async Task<bool> Delete(FoodGrouping FoodGrouping)
        {
            await DataContext.FoodGrouping.Where(x => x.Id == FoodGrouping.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<FoodGrouping> FoodGroupings)
        {
            List<FoodGroupingDAO> FoodGroupingDAOs = new List<FoodGroupingDAO>();
            foreach (FoodGrouping FoodGrouping in FoodGroupings)
            {
                FoodGroupingDAO FoodGroupingDAO = new FoodGroupingDAO();
                FoodGroupingDAO.Id = FoodGrouping.Id;
                FoodGroupingDAO.Name = FoodGrouping.Name;
                FoodGroupingDAO.StatusId = FoodGrouping.StatusId;
                FoodGroupingDAOs.Add(FoodGroupingDAO);
            }
            await DataContext.BulkMergeAsync(FoodGroupingDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<FoodGrouping> FoodGroupings)
        {
            List<long> Ids = FoodGroupings.Select(x => x.Id).ToList();
            await DataContext.FoodGrouping
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(FoodGrouping FoodGrouping)
        {
        }
        
    }
}
