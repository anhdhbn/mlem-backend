using Common;
using Helpers;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MM.Common;
using MM.Entities;
using MM.Repositories;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MM.Services.MAccount
{
    public interface IAccountService :  IServiceScoped
    {
        Task<int> Count(AccountFilter AccountFilter);
        Task<List<Account>> List(AccountFilter AccountFilter);
        Task<Account> Get(long Id);
        Task<Account> Login(Account Account);
        Task<Account> Create(Account Account);
        Task<Account> Update(Account Account);
        Task<Account> ChangePassword(Account Account);
        Task<Account> ForgotPassword(Account Account);
        Task<Account> VerifyCode(Account Account);
        Task<Account> RecoveryPassword(Account Account);
        string GenerateCode();
    }

    public class AccountService : IAccountService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IAccountValidator AccountValidator;
        private IConfiguration Configuration;

        public AccountService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IAccountValidator AccountValidator,
            IConfiguration Configuration
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.AccountValidator = AccountValidator;
            this.Configuration = Configuration;
        }
        public async Task<int> Count(AccountFilter AccountFilter)
        {
            try
            {
                int result = await UOW.AccountRepository.Count(AccountFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(AccountService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Account>> List(AccountFilter AccountFilter)
        {
            try
            {
                List<Account> Accounts = await UOW.AccountRepository.List(AccountFilter);
                return Accounts;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(AccountService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Account> Get(long Id)
        {
            Account Account = await UOW.AccountRepository.Get(Id);
            if (Account == null)
                return null;
            return Account;
        }
       
        public async Task<Account> Create(Account Account)
        {
            if (!await AccountValidator.Create(Account))
                return Account;

            try
            {
                await UOW.Begin();
                Account.Email = Account.Email.ToLower();
                Account.Salt = GenerateSalt();
                Account.Password = HashPassword(Account.Password, Account.Salt);
                Account.RoleId = Enums.AccountRole.USER.Id;
                await UOW.AccountRepository.Create(Account);
                await UOW.Commit();

                return await UOW.AccountRepository.Get(Account.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Account> Update(Account Account)
        {
            if (!await AccountValidator.Update(Account))
                return Account;
            try
            {
                var oldData = await UOW.AccountRepository.Get(Account.Id);
                Account.Password = oldData.Password;
                Account.PasswordRecoveryCode = oldData.PasswordRecoveryCode;
                Account.ExpiredTimeCode = oldData.ExpiredTimeCode;
                Account.Salt = oldData.Salt;

                await UOW.Begin();
                await UOW.AccountRepository.Update(Account);
                await UOW.Commit();

                var newData = await UOW.AccountRepository.Get(Account.Id);
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Account> Login(Account Account)
        {
            if (!await AccountValidator.Login(Account))
                return Account;
            Account = await UOW.AccountRepository.Get(Account.Id);
            Account.Token = CreateToken(Account.Id, Account.Email);
            return Account;
        }

        public async Task<Account> ChangePassword(Account Account)
        {
            if (!await AccountValidator.ChangePassword(Account))
                return Account;

            try
            {
                Account old = await UOW.AccountRepository.Get(Account.Id);
                old.Salt = GenerateSalt();
                old.Password = HashPassword(old.NewPassword, old.Salt);
                await UOW.Begin();
                await UOW.AccountRepository.Update(old);
                await UOW.Commit();

                var obj = await UOW.AccountRepository.Get(Account.Id);
                return obj;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Account> ForgotPassword(Account Account)
        {
            try
            {
                AccountFilter AccountFilter = new AccountFilter
                {
                    Skip = 0,
                    Take = 1,
                    Email = new StringFilter { Equal = Account.Email.ToLower() },
                    Selects = AccountSelect.ALL
                };

                Account = (await UOW.AccountRepository.List(AccountFilter)).FirstOrDefault();
                Account.PasswordRecoveryCode = GenerateCode();
                Account.ExpiredTimeCode = (DateTime.Now).AddSeconds(1800);

                await UOW.Begin();
                await UOW.AccountRepository.Update(Account);
                await UOW.Commit();

                var obj = await UOW.AccountRepository.Get(Account.Id);
                return obj;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Account> VerifyCode(Account Account)
        {
            try
            {
                await UOW.Begin();
                await UOW.AccountRepository.Update(Account);
                await UOW.Commit();

                var obj = await UOW.AccountRepository.Get(Account.Id);
                return obj;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Account> RecoveryPassword(Account Account)
        {
            try
            {
                Account.Salt = GenerateSalt();
                Account.Password = HashPassword(Account.Password, Account.Salt);
                await UOW.Begin();
                await UOW.AccountRepository.Update(Account);
                await UOW.Commit();

                var obj = await UOW.AccountRepository.Get(Account.Id);
                return obj;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        private string CreateToken(long Id, string Email)
        {
            var secretKey = Configuration["Config:SecretKey"];
            var expiredTime = double.TryParse(Configuration["Config:ExpiredTime"], out double time) ? time : 0;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
                    new Claim(ClaimTypes.Name, Email)
                }),
                Expires = StaticParams.DateTimeNow.AddSeconds(expiredTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken SecurityToken = tokenHandler.CreateToken(tokenDescriptor);
            string Token = tokenHandler.WriteToken(SecurityToken);
            return Token;
        }

        private string HashPassword(string password, string Salt)
        {
            Salt = string.IsNullOrEmpty(Salt) ? string.Empty : Salt;
            byte[] salt = Encoding.ASCII.GetBytes(Salt);
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }

        private string GenerateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt.ToString();
        }

        public string GenerateCode()
        {
            const string valid = "1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            for (int i = 0; i < 4; i++)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
    }
}
