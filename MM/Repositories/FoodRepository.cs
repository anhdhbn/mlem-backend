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
    public interface IFoodRepository
    {
        Task<int> Count(FoodFilter FoodFilter);
        Task<List<Food>> List(FoodFilter FoodFilter);
        Task<Food> Get(long Id);
        Task<bool> Create(Food Food);
        Task<bool> Update(Food Food);
        Task<bool> Delete(Food Food);
        Task<bool> BulkMerge(List<Food> Foods);
        Task<bool> BulkDelete(List<Food> Foods);
    }
    public class FoodRepository : IFoodRepository
    {
        private DataContext DataContext;
        public FoodRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<FoodDAO> DynamicFilter(IQueryable<FoodDAO> query, FoodFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.PriceEach != null)
                query = query.Where(q => q.PriceEach, filter.PriceEach);
            if (filter.DiscountRate != null)
                query = query.Where(q => q.DiscountRate, filter.DiscountRate);
            if (filter.Image != null)
                query = query.Where(q => q.Image, filter.Image);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.Descreption != null)
                query = query.Where(q => q.Descreption, filter.Descreption);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<FoodDAO> OrFilter(IQueryable<FoodDAO> query, FoodFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<FoodDAO> initQuery = query.Where(q => false);
            foreach (FoodFilter FoodFilter in filter.OrFilter)
            {
                IQueryable<FoodDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.PriceEach != null)
                    queryable = queryable.Where(q => q.PriceEach, filter.PriceEach);
                if (filter.DiscountRate != null)
                    queryable = queryable.Where(q => q.DiscountRate, filter.DiscountRate);
                if (filter.Image != null)
                    queryable = queryable.Where(q => q.Image, filter.Image);
                if (filter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                if (filter.Descreption != null)
                    queryable = queryable.Where(q => q.Descreption, filter.Descreption);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<FoodDAO> DynamicOrder(IQueryable<FoodDAO> query, FoodFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case FoodOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case FoodOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case FoodOrder.PriceEach:
                            query = query.OrderBy(q => q.PriceEach);
                            break;
                        case FoodOrder.DiscountRate:
                            query = query.OrderBy(q => q.DiscountRate);
                            break;
                        case FoodOrder.Image:
                            query = query.OrderBy(q => q.Image);
                            break;
                        case FoodOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case FoodOrder.Descreption:
                            query = query.OrderBy(q => q.Descreption);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case FoodOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case FoodOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case FoodOrder.PriceEach:
                            query = query.OrderByDescending(q => q.PriceEach);
                            break;
                        case FoodOrder.DiscountRate:
                            query = query.OrderByDescending(q => q.DiscountRate);
                            break;
                        case FoodOrder.Image:
                            query = query.OrderByDescending(q => q.Image);
                            break;
                        case FoodOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case FoodOrder.Descreption:
                            query = query.OrderByDescending(q => q.Descreption);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Food>> DynamicSelect(IQueryable<FoodDAO> query, FoodFilter filter)
        {
            List<Food> Foods = await query.Select(q => new Food()
            {
                Id = filter.Selects.Contains(FoodSelect.Id) ? q.Id : default(long),
                Name = filter.Selects.Contains(FoodSelect.Name) ? q.Name : default(string),
                PriceEach = filter.Selects.Contains(FoodSelect.PriceEach) ? q.PriceEach : default(decimal),
                DiscountRate = filter.Selects.Contains(FoodSelect.DiscountRate) ? q.DiscountRate : default(decimal?),
                Image = filter.Selects.Contains(FoodSelect.Image) ? q.Image : default(string),
                StatusId = filter.Selects.Contains(FoodSelect.Status) ? q.StatusId : default(long),
                Descreption = filter.Selects.Contains(FoodSelect.Descreption) ? q.Descreption : default(string),
            }).AsNoTracking().ToListAsync();
            return Foods;
        }

        public async Task<int> Count(FoodFilter filter)
        {
            IQueryable<FoodDAO> Foods = DataContext.Food;
            Foods = DynamicFilter(Foods, filter);
            return await Foods.CountAsync();
        }

        public async Task<List<Food>> List(FoodFilter filter)
        {
            if (filter == null) return new List<Food>();
            IQueryable<FoodDAO> FoodDAOs = DataContext.Food;
            FoodDAOs = DynamicFilter(FoodDAOs, filter);
            FoodDAOs = DynamicOrder(FoodDAOs, filter);
            List<Food> Foods = await DynamicSelect(FoodDAOs, filter);
            return Foods;
        }

        public async Task<Food> Get(long Id)
        {
            Food Food = await DataContext.Food.Where(x => x.Id == Id).Select(x => new Food()
            {
                Id = x.Id,
                Name = x.Name,
                PriceEach = x.PriceEach,
                DiscountRate = x.DiscountRate,
                Image = x.Image,
                StatusId = x.StatusId,
                Descreption = x.Descreption,
            }).AsNoTracking().FirstOrDefaultAsync();

            if (Food == null)
                return null;

            return Food;
        }
        public async Task<bool> Create(Food Food)
        {
            FoodDAO FoodDAO = new FoodDAO();
            FoodDAO.Id = Food.Id;
            FoodDAO.Name = Food.Name;
            FoodDAO.PriceEach = Food.PriceEach;
            FoodDAO.DiscountRate = Food.DiscountRate;
            FoodDAO.Image = Food.Image;
            FoodDAO.StatusId = Food.StatusId;
            FoodDAO.Descreption = Food.Descreption;
            FoodDAO.CreatedAt = StaticParams.DateTimeNow;
            FoodDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Food.Add(FoodDAO);
            await DataContext.SaveChangesAsync();
            Food.Id = FoodDAO.Id;
            await SaveReference(Food);
            return true;
        }

        public async Task<bool> Update(Food Food)
        {
            FoodDAO FoodDAO = DataContext.Food.Where(x => x.Id == Food.Id).FirstOrDefault();
            if (FoodDAO == null)
                return false;
            FoodDAO.Id = Food.Id;
            FoodDAO.Name = Food.Name;
            FoodDAO.PriceEach = Food.PriceEach;
            FoodDAO.DiscountRate = Food.DiscountRate;
            FoodDAO.Image = Food.Image;
            FoodDAO.StatusId = Food.StatusId;
            FoodDAO.Descreption = Food.Descreption;
            FoodDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Food);
            return true;
        }

        public async Task<bool> Delete(Food Food)
        {
            await DataContext.Food.Where(x => x.Id == Food.Id).UpdateFromQueryAsync(x => new FoodDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Food> Foods)
        {
            List<FoodDAO> FoodDAOs = new List<FoodDAO>();
            foreach (Food Food in Foods)
            {
                FoodDAO FoodDAO = new FoodDAO();
                FoodDAO.Id = Food.Id;
                FoodDAO.Name = Food.Name;
                FoodDAO.PriceEach = Food.PriceEach;
                FoodDAO.DiscountRate = Food.DiscountRate;
                FoodDAO.Image = Food.Image;
                FoodDAO.StatusId = Food.StatusId;
                FoodDAO.Descreption = Food.Descreption;
                FoodDAO.CreatedAt = StaticParams.DateTimeNow;
                FoodDAO.UpdatedAt = StaticParams.DateTimeNow;
                FoodDAOs.Add(FoodDAO);
            }
            await DataContext.BulkMergeAsync(FoodDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Food> Foods)
        {
            List<long> Ids = Foods.Select(x => x.Id).ToList();
            await DataContext.Food
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new FoodDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Food Food)
        {
        }
        
    }
}
