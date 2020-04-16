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
    public interface IAccountFoodFavoriteRepository
    {
        Task<int> Count(AccountFoodFavoriteFilter AccountFoodFavoriteFilter);
        Task<List<AccountFoodFavorite>> List(AccountFoodFavoriteFilter AccountFoodFavoriteFilter);
        Task<bool> BulkMerge(List<AccountFoodFavorite> AccountFoodFavorites);
    }
    public class AccountFoodFavoriteRepository : IAccountFoodFavoriteRepository
    {
        private DataContext DataContext;
        public AccountFoodFavoriteRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<AccountFoodFavoriteDAO> DynamicFilter(IQueryable<AccountFoodFavoriteDAO> query, AccountFoodFavoriteFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.AccountId != null)
                query = query.Where(q => q.AccountId, filter.AccountId);
            if (filter.FoodId != null)
                query = query.Where(q => q.FoodId, filter.FoodId);
            return query;
        }

        private IQueryable<AccountFoodFavoriteDAO> DynamicOrder(IQueryable<AccountFoodFavoriteDAO> query, AccountFoodFavoriteFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case AccountFoodFavoriteOrder.Account:
                            query = query.OrderBy(q => q.AccountId);
                            break;
                        case AccountFoodFavoriteOrder.Food:
                            query = query.OrderBy(q => q.FoodId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case AccountFoodFavoriteOrder.Account:
                            query = query.OrderByDescending(q => q.AccountId);
                            break;
                        case AccountFoodFavoriteOrder.Food:
                            query = query.OrderByDescending(q => q.FoodId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<AccountFoodFavorite>> DynamicSelect(IQueryable<AccountFoodFavoriteDAO> query, AccountFoodFavoriteFilter filter)
        {
            List<AccountFoodFavorite> AccountFoodFavorites = await query.Select(q => new AccountFoodFavorite()
            {
                AccountId = filter.Selects.Contains(AccountFoodFavoriteSelect.Account) ? q.AccountId : default(long),
                FoodId = filter.Selects.Contains(AccountFoodFavoriteSelect.Food) ? q.FoodId : default(long),
                Account = filter.Selects.Contains(AccountFoodFavoriteSelect.Account) && q.Account != null ? new Account
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
                Food = filter.Selects.Contains(AccountFoodFavoriteSelect.Food) && q.Food != null ? new Food
                {
                    Id = q.Food.Id,
                    Name = q.Food.Name,
                    PriceEach = q.Food.PriceEach,
                    DiscountRate = q.Food.DiscountRate,
                    Image = q.Food.Image,
                    StatusId = q.Food.StatusId,
                    Descreption = q.Food.Descreption,
                } : null,
            }).AsNoTracking().ToListAsync();
            return AccountFoodFavorites;
        }

        public async Task<int> Count(AccountFoodFavoriteFilter filter)
        {
            IQueryable<AccountFoodFavoriteDAO> AccountFoodFavorites = DataContext.AccountFoodFavorite;
            AccountFoodFavorites = DynamicFilter(AccountFoodFavorites, filter);
            return await AccountFoodFavorites.CountAsync();
        }

        public async Task<List<AccountFoodFavorite>> List(AccountFoodFavoriteFilter filter)
        {
            if (filter == null) return new List<AccountFoodFavorite>();
            IQueryable<AccountFoodFavoriteDAO> AccountFoodFavoriteDAOs = DataContext.AccountFoodFavorite;
            AccountFoodFavoriteDAOs = DynamicFilter(AccountFoodFavoriteDAOs, filter);
            AccountFoodFavoriteDAOs = DynamicOrder(AccountFoodFavoriteDAOs, filter);
            List<AccountFoodFavorite> AccountFoodFavorites = await DynamicSelect(AccountFoodFavoriteDAOs, filter);
            return AccountFoodFavorites;
        }
        
        public async Task<bool> BulkMerge(List<AccountFoodFavorite> AccountFoodFavorites)
        {
            List<AccountFoodFavoriteDAO> AccountFoodFavoriteDAOs = new List<AccountFoodFavoriteDAO>();
            foreach (AccountFoodFavorite AccountFoodFavorite in AccountFoodFavorites)
            {
                AccountFoodFavoriteDAO AccountFoodFavoriteDAO = new AccountFoodFavoriteDAO();
                AccountFoodFavoriteDAO.AccountId = AccountFoodFavorite.AccountId;
                AccountFoodFavoriteDAO.FoodId = AccountFoodFavorite.FoodId;
                AccountFoodFavoriteDAOs.Add(AccountFoodFavoriteDAO);
            }
            await DataContext.BulkMergeAsync(AccountFoodFavoriteDAOs);
            return true;
        }

        private async Task SaveReference(AccountFoodFavorite AccountFoodFavorite)
        {
        }
        
    }
}
