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
    public interface IAccountRepository
    {
        Task<int> Count(AccountFilter AccountFilter);
        Task<List<Account>> List(AccountFilter AccountFilter);
        Task<Account> Get(long Id);
        Task<bool> Create(Account Account);
        Task<bool> Update(Account Account);
    }
    public class AccountRepository : IAccountRepository
    {
        private DataContext DataContext;
        public AccountRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<AccountDAO> DynamicFilter(IQueryable<AccountDAO> query, AccountFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.DisplayName != null)
                query = query.Where(q => q.DisplayName, filter.DisplayName);
            if (filter.Email != null)
                query = query.Where(q => q.Email, filter.Email);
            if (filter.Phone != null)
                query = query.Where(q => q.Phone, filter.Phone);
            if (filter.Password != null)
                query = query.Where(q => q.Password, filter.Password);
            if (filter.Salt != null)
                query = query.Where(q => q.Salt, filter.Salt);
            if (filter.PasswordRecoveryCode != null)
                query = query.Where(q => q.PasswordRecoveryCode, filter.PasswordRecoveryCode);
            if (filter.ExpiredTimeCode != null)
                query = query.Where(q => q.ExpiredTimeCode, filter.ExpiredTimeCode);
            if (filter.Address != null)
                query = query.Where(q => q.Address, filter.Address);
            if (filter.Dob != null)
                query = query.Where(q => q.Dob, filter.Dob);
            if (filter.Avatar != null)
                query = query.Where(q => q.Avatar, filter.Avatar);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<AccountDAO> OrFilter(IQueryable<AccountDAO> query, AccountFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<AccountDAO> initQuery = query.Where(q => false);
            foreach (AccountFilter AccountFilter in filter.OrFilter)
            {
                IQueryable<AccountDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.DisplayName != null)
                    queryable = queryable.Where(q => q.DisplayName, filter.DisplayName);
                if (filter.Email != null)
                    queryable = queryable.Where(q => q.Email, filter.Email);
                if (filter.Phone != null)
                    queryable = queryable.Where(q => q.Phone, filter.Phone);
                if (filter.Password != null)
                    queryable = queryable.Where(q => q.Password, filter.Password);
                if (filter.Salt != null)
                    queryable = queryable.Where(q => q.Salt, filter.Salt);
                if (filter.PasswordRecoveryCode != null)
                    queryable = queryable.Where(q => q.PasswordRecoveryCode, filter.PasswordRecoveryCode);
                if (filter.ExpiredTimeCode != null)
                    queryable = queryable.Where(q => q.ExpiredTimeCode, filter.ExpiredTimeCode);
                if (filter.Address != null)
                    queryable = queryable.Where(q => q.Address, filter.Address);
                if (filter.Dob != null)
                    queryable = queryable.Where(q => q.Dob, filter.Dob);
                if (filter.Avatar != null)
                    queryable = queryable.Where(q => q.Avatar, filter.Avatar);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<AccountDAO> DynamicOrder(IQueryable<AccountDAO> query, AccountFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case AccountOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case AccountOrder.DisplayName:
                            query = query.OrderBy(q => q.DisplayName);
                            break;
                        case AccountOrder.Email:
                            query = query.OrderBy(q => q.Email);
                            break;
                        case AccountOrder.Phone:
                            query = query.OrderBy(q => q.Phone);
                            break;
                        case AccountOrder.Address:
                            query = query.OrderBy(q => q.Address);
                            break;
                        case AccountOrder.Dob:
                            query = query.OrderBy(q => q.Dob);
                            break;
                        case AccountOrder.Avatar:
                            query = query.OrderBy(q => q.Avatar);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case AccountOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case AccountOrder.DisplayName:
                            query = query.OrderByDescending(q => q.DisplayName);
                            break;
                        case AccountOrder.Email:
                            query = query.OrderByDescending(q => q.Email);
                            break;
                        case AccountOrder.Phone:
                            query = query.OrderByDescending(q => q.Phone);
                            break;
                        case AccountOrder.Address:
                            query = query.OrderByDescending(q => q.Address);
                            break;
                        case AccountOrder.Dob:
                            query = query.OrderByDescending(q => q.Dob);
                            break;
                        case AccountOrder.Avatar:
                            query = query.OrderByDescending(q => q.Avatar);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Account>> DynamicSelect(IQueryable<AccountDAO> query, AccountFilter filter)
        {
            List<Account> Accounts = await query.Select(q => new Account()
            {
                Id = filter.Selects.Contains(AccountSelect.Id) ? q.Id : default(long),
                DisplayName = filter.Selects.Contains(AccountSelect.DisplayName) ? q.DisplayName : default(string),
                Email = filter.Selects.Contains(AccountSelect.Email) ? q.Email : default(string),
                Phone = filter.Selects.Contains(AccountSelect.Phone) ? q.Phone : default(string),
                Password = filter.Selects.Contains(AccountSelect.Password) ? q.Password : default(string),
                Salt = filter.Selects.Contains(AccountSelect.Salt) ? q.Salt : default(string),
                PasswordRecoveryCode = filter.Selects.Contains(AccountSelect.PasswordRecoveryCode) ? q.PasswordRecoveryCode : default(string),
                ExpiredTimeCode = filter.Selects.Contains(AccountSelect.ExpiredTimeCode) ? q.ExpiredTimeCode : default(DateTime?),
                Address = filter.Selects.Contains(AccountSelect.Address) ? q.Address : default(string),
                Dob = filter.Selects.Contains(AccountSelect.Dob) ? q.Dob : default(DateTime?),
                Avatar = filter.Selects.Contains(AccountSelect.Avatar) ? q.Avatar : default(string),
                RoleId = filter.Selects.Contains(AccountSelect.Role) ? q.RoleId : default(long),
            }).AsNoTracking().ToListAsync();
            return Accounts;
        }

        public async Task<int> Count(AccountFilter filter)
        {
            IQueryable<AccountDAO> Accounts = DataContext.Account;
            Accounts = DynamicFilter(Accounts, filter);
            return await Accounts.CountAsync();
        }

        public async Task<List<Account>> List(AccountFilter filter)
        {
            if (filter == null) return new List<Account>();
            IQueryable<AccountDAO> AccountDAOs = DataContext.Account;
            AccountDAOs = DynamicFilter(AccountDAOs, filter);
            AccountDAOs = DynamicOrder(AccountDAOs, filter);
            List<Account> Accounts = await DynamicSelect(AccountDAOs, filter);
            return Accounts;
        }

        public async Task<Account> Get(long Id)
        {
            Account Account = await DataContext.Account.Where(x => x.Id == Id).Select(x => new Account()
            {
                Id = x.Id,
                DisplayName = x.DisplayName,
                Email = x.Email,
                Phone = x.Phone,
                Password = x.Password,
                Salt = x.Salt,
                PasswordRecoveryCode = x.PasswordRecoveryCode,
                ExpiredTimeCode = x.ExpiredTimeCode,
                Address = x.Address,
                Dob = x.Dob,
                Avatar = x.Avatar,
                RoleId = x.RoleId,
            }).AsNoTracking().FirstOrDefaultAsync();

            if (Account == null)
                return null;

            Account.AccountFoodFavorites = await DataContext.AccountFoodFavorite.Where(x => x.AccountId == Id)
                .Select(x => new AccountFoodFavorite
                {
                    AccountId = x.AccountId,
                    FoodId = x.FoodId,
                    Food = new Food
                    {
                        Id = x.Food.Id,
                        Name = x.Food.Name,
                        Descreption = x.Food.Descreption,
                        PriceEach = x.Food.PriceEach,
                        DiscountRate = x.Food.DiscountRate,
                        Image = x.Food.Image
                    }
                }).ToListAsync();

            Account.Orders = await DataContext.Order.Where(x => x.AccountId == Id)
                .Select(x => new Order
                {
                    AccountId = x.AccountId,
                    Code = x.Code,
                    Descreption = x.Descreption,
                    Id = x.Id,
                    NumOfPerson = x.NumOfPerson,
                    NumOfTable = x.NumOfTable,
                    OrderDate = x.OrderDate,
                    PayDate = x.PayDate,
                    StatusId = x.StatusId
                }).ToListAsync();
            return Account;
        }
        public async Task<bool> Create(Account Account)
        {
            AccountDAO AccountDAO = new AccountDAO();
            AccountDAO.Id = Account.Id;
            AccountDAO.DisplayName = Account.DisplayName;
            AccountDAO.Email = Account.Email;
            AccountDAO.Phone = Account.Phone;
            AccountDAO.Password = Account.Password;
            AccountDAO.Salt = Account.Salt;
            AccountDAO.PasswordRecoveryCode = Account.PasswordRecoveryCode;
            AccountDAO.ExpiredTimeCode = Account.ExpiredTimeCode;
            AccountDAO.Address = Account.Address;
            AccountDAO.Dob = Account.Dob;
            AccountDAO.Avatar = Account.Avatar;
            AccountDAO.RoleId = Account.RoleId;
            AccountDAO.CreatedAt = StaticParams.DateTimeNow;
            AccountDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Account.Add(AccountDAO);
            await DataContext.SaveChangesAsync();
            Account.Id = AccountDAO.Id;
            await SaveReference(Account);
            return true;
        }

        public async Task<bool> Update(Account Account)
        {
            AccountDAO AccountDAO = DataContext.Account.Where(x => x.Id == Account.Id).FirstOrDefault();
            if (AccountDAO == null)
                return false;
            AccountDAO.Id = Account.Id;
            AccountDAO.DisplayName = Account.DisplayName;
            AccountDAO.Phone = Account.Phone;
            AccountDAO.Password = Account.Password;
            AccountDAO.Salt = Account.Salt;
            AccountDAO.PasswordRecoveryCode = Account.PasswordRecoveryCode;
            AccountDAO.ExpiredTimeCode = Account.ExpiredTimeCode;
            AccountDAO.Address = Account.Address;
            AccountDAO.Dob = Account.Dob;
            AccountDAO.Avatar = Account.Avatar;
            AccountDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Account);
            return true;
        }

        private async Task SaveReference(Account Account)
        {
            await DataContext.AccountFoodFavorite.Where(a => a.AccountId == Account.Id).DeleteFromQueryAsync();
            if(Account.AccountFoodFavorites != null)
            {
                List<AccountFoodFavoriteDAO> AccountFoodFavoriteDAOs = new List<AccountFoodFavoriteDAO>();
                foreach (var AccountFoodFavorite in Account.AccountFoodFavorites)
                {
                    AccountFoodFavoriteDAO AccountFoodFavoriteDAO = new AccountFoodFavoriteDAO
                    {
                        AccountId = Account.Id,
                        FoodId = AccountFoodFavorite.FoodId
                    };
                    AccountFoodFavoriteDAOs.Add(AccountFoodFavoriteDAO);
                }
                DataContext.AccountFoodFavorite.AddRange(AccountFoodFavoriteDAOs);
            }
            await DataContext.SaveChangesAsync();
        }
    }
}
