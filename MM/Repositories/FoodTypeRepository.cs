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
    public interface IFoodTypeRepository
    {
        Task<int> Count(FoodTypeFilter FoodTypeFilter);
        Task<List<FoodType>> List(FoodTypeFilter FoodTypeFilter);
        Task<FoodType> Get(long Id);
        Task<bool> Create(FoodType FoodType);
        Task<bool> Update(FoodType FoodType);
        Task<bool> Delete(FoodType FoodType);
        Task<bool> BulkMerge(List<FoodType> FoodTypes);
        Task<bool> BulkDelete(List<FoodType> FoodTypes);
    }
    public class FoodTypeRepository : IFoodTypeRepository
    {
        private DataContext DataContext;
        public FoodTypeRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<FoodTypeDAO> DynamicFilter(IQueryable<FoodTypeDAO> query, FoodTypeFilter filter)
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

         private IQueryable<FoodTypeDAO> OrFilter(IQueryable<FoodTypeDAO> query, FoodTypeFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<FoodTypeDAO> initQuery = query.Where(q => false);
            foreach (FoodTypeFilter FoodTypeFilter in filter.OrFilter)
            {
                IQueryable<FoodTypeDAO> queryable = query;
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

        private IQueryable<FoodTypeDAO> DynamicOrder(IQueryable<FoodTypeDAO> query, FoodTypeFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case FoodTypeOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case FoodTypeOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case FoodTypeOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case FoodTypeOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case FoodTypeOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case FoodTypeOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<FoodType>> DynamicSelect(IQueryable<FoodTypeDAO> query, FoodTypeFilter filter)
        {
            List<FoodType> FoodTypes = await query.Select(q => new FoodType()
            {
                Id = filter.Selects.Contains(FoodTypeSelect.Id) ? q.Id : default(long),
                Name = filter.Selects.Contains(FoodTypeSelect.Name) ? q.Name : default(string),
                StatusId = filter.Selects.Contains(FoodTypeSelect.Status) ? q.StatusId : default(long),
            }).AsNoTracking().ToListAsync();
            return FoodTypes;
        }

        public async Task<int> Count(FoodTypeFilter filter)
        {
            IQueryable<FoodTypeDAO> FoodTypes = DataContext.FoodType;
            FoodTypes = DynamicFilter(FoodTypes, filter);
            return await FoodTypes.CountAsync();
        }

        public async Task<List<FoodType>> List(FoodTypeFilter filter)
        {
            if (filter == null) return new List<FoodType>();
            IQueryable<FoodTypeDAO> FoodTypeDAOs = DataContext.FoodType;
            FoodTypeDAOs = DynamicFilter(FoodTypeDAOs, filter);
            FoodTypeDAOs = DynamicOrder(FoodTypeDAOs, filter);
            List<FoodType> FoodTypes = await DynamicSelect(FoodTypeDAOs, filter);
            return FoodTypes;
        }

        public async Task<FoodType> Get(long Id)
        {
            FoodType FoodType = await DataContext.FoodType.Where(x => x.Id == Id).Select(x => new FoodType()
            {
                Id = x.Id,
                Name = x.Name,
                StatusId = x.StatusId,
            }).AsNoTracking().FirstOrDefaultAsync();

            if (FoodType == null)
                return null;

            return FoodType;
        }
        public async Task<bool> Create(FoodType FoodType)
        {
            FoodTypeDAO FoodTypeDAO = new FoodTypeDAO();
            FoodTypeDAO.Id = FoodType.Id;
            FoodTypeDAO.Name = FoodType.Name;
            FoodTypeDAO.StatusId = FoodType.StatusId;
            DataContext.FoodType.Add(FoodTypeDAO);
            await DataContext.SaveChangesAsync();
            FoodType.Id = FoodTypeDAO.Id;
            await SaveReference(FoodType);
            return true;
        }

        public async Task<bool> Update(FoodType FoodType)
        {
            FoodTypeDAO FoodTypeDAO = DataContext.FoodType.Where(x => x.Id == FoodType.Id).FirstOrDefault();
            if (FoodTypeDAO == null)
                return false;
            FoodTypeDAO.Id = FoodType.Id;
            FoodTypeDAO.Name = FoodType.Name;
            FoodTypeDAO.StatusId = FoodType.StatusId;
            await DataContext.SaveChangesAsync();
            await SaveReference(FoodType);
            return true;
        }

        public async Task<bool> Delete(FoodType FoodType)
        {
            await DataContext.FoodType.Where(x => x.Id == FoodType.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<FoodType> FoodTypes)
        {
            List<FoodTypeDAO> FoodTypeDAOs = new List<FoodTypeDAO>();
            foreach (FoodType FoodType in FoodTypes)
            {
                FoodTypeDAO FoodTypeDAO = new FoodTypeDAO();
                FoodTypeDAO.Id = FoodType.Id;
                FoodTypeDAO.Name = FoodType.Name;
                FoodTypeDAO.StatusId = FoodType.StatusId;
                FoodTypeDAOs.Add(FoodTypeDAO);
            }
            await DataContext.BulkMergeAsync(FoodTypeDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<FoodType> FoodTypes)
        {
            List<long> Ids = FoodTypes.Select(x => x.Id).ToList();
            await DataContext.FoodType
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(FoodType FoodType)
        {
        }
        
    }
}
