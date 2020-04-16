using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using MM.Entities;
using MM;
using MM.Repositories;
using MM.Common;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace MM.Services.MAccount
{
    public interface IAccountValidator : IServiceScoped
    {
        Task<bool> Login(Account Account);
        Task<bool> ChangePassword(Account Account);
        Task<bool> Create(Account Account);
        Task<bool> Update(Account Account);
    }

    public class AccountValidator : IAccountValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            EmailNotExisted,
            EmailExisted,
            EmailInvalid,
            PhoneEmpty,
            PasswordNotMatch
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public AccountValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Account Account)
        {
            AccountFilter AccountFilter = new AccountFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Account.Id },
                Selects = AccountSelect.Id
            };

            int count = await UOW.AccountRepository.Count(AccountFilter);
            if (count == 0)
                Account.AddError(nameof(AccountValidator), nameof(Account.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }
        private async Task<bool> ValidateEmail(Account Account)
        {
            if (!IsValidEmail(Account.Email))
            {
                Account.AddError(nameof(AccountValidator), nameof(Account.Email), ErrorCode.EmailInvalid);
                return false;
            }
            List<Account> Accounts = await UOW.AccountRepository.List(new AccountFilter
            {
                Skip = 0,
                Take = 1,
                Email = new StringFilter { Equal = Account.Email },
                Selects = AccountSelect.Email,
            });
            if (Accounts.Count > 0)
            {
                Account.AddError(nameof(AccountValidator), nameof(Account.Email), ErrorCode.EmailExisted);
                return false;
            }

            return Account.IsValidated;
        }

        private async Task<bool> ValidatePhone(Account Account)
        {
            if (string.IsNullOrEmpty(Account.Phone))
            {
                Account.AddError(nameof(AccountValidator), nameof(Account.Phone), ErrorCode.PhoneEmpty);
            }
            return Account.IsValidated;
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Login(Account Account)
        {
            List<Account> Accounts = await UOW.AccountRepository.List(new AccountFilter
            {
                Skip = 0,
                Take = 1,
                Email = new StringFilter { Equal = Account.Email },
                Selects = AccountSelect.ALL,
            });
            if (Accounts.Count == 0)
            {
                Account.AddError(nameof(AccountValidator), nameof(Account.Email), ErrorCode.EmailNotExisted);
            }
            else
            {
                Account account = Accounts.FirstOrDefault();
                bool verify = VerifyPassword(account.Password, Account.Password, account.Salt);
                if (verify == false)
                {
                    Account.AddError(nameof(AccountValidator), nameof(Account.Password), ErrorCode.PasswordNotMatch);
                }
                else
                {
                    Account.Id = account.Id;
                }
            }
            return Account.IsValidated;
        }

        public async Task<bool> ChangePassword(Account Account)
        {
            List<Account> Accounts = await UOW.AccountRepository.List(new AccountFilter
            {
                Skip = 0,
                Take = 1,
                Id = new IdFilter { Equal = Account.Id },
                Selects = AccountSelect.ALL,
            });
            if (Accounts.Count == 0)
            {
                Account.AddError(nameof(AccountValidator), nameof(Account.Email), ErrorCode.IdNotExisted);
            }
            else
            {
                Account account = Accounts.FirstOrDefault();
                bool verify = VerifyPassword(account.Password, Account.Password, account.Salt);
                if (verify == false)
                {
                    Account.AddError(nameof(AccountValidator), nameof(Account.Password), ErrorCode.PasswordNotMatch);
                }
            }
            return Account.IsValidated;
        }

        private bool VerifyPassword(string source, string password, string Salt)
        {
            Salt = string.IsNullOrEmpty(Salt) ? string.Empty : Salt;
            byte[] salt = Encoding.ASCII.GetBytes(Salt);
            var hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            if (!hashedPassword.Equals(source))
            {
                return false;
            }
            return true;
        }

        public async Task<bool>Create(Account Account)
        {
            await ValidateEmail(Account);
            await ValidatePhone(Account);
            return Account.IsValidated;
        }

        public async Task<bool> Update(Account Account)
        {
            if (await ValidateId(Account))
            {
                await ValidatePhone(Account);
            }
            return Account.IsValidated;
        }
    }
}
