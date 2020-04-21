using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MM.Services.MAccount;
using MM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using MM.Services;
using System.Security.Claims;
using MM.Common;
using Helpers;

namespace MM.Controller.account
{
    public class AccountRoute : Root
    {
        private const string Default = Api + "/account";
        public const string Login = Default + "/login";
        public const string Get = Default + "/get";
        public const string Register = Default + "/register";
        public const string Update = Default + "/update";
        public const string LikeFood = Default + "/like-food";
        public const string ChangePassword = Default + "/change-password";
        public const string ForgotPassword = Default + "/forgot-password";
        public const string RecoveryPassword = Default + "/recovery-password";
        public const string VerifyCode = Default + "/verify-code";
    }
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService AccountService;
        private readonly IMailService MailService;
        //private string clientId = StaticParams.GoogleClientId;
        //private string clientSecret = StaticParams.GoogleClientSecret;
        //private string redirectUri = StaticParams.GoogleRedirectUri;
        private string grant_type = "authorization_code";
        public AccountController(IAccountService AccountService, IMailService MailService)
        {
            this.AccountService = AccountService;
            this.MailService = MailService;
        }

        #region Login
        [AllowAnonymous]
        [Route(AccountRoute.Login), HttpPost]
        public async Task<ActionResult<Account_AccountDTO>> Login([FromBody] Account_LoginDTO Account_LoginDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Account Account = new Account
            {
                Email = Account_LoginDTO.Email,
                Password = Account_LoginDTO.Password
            };
            Account = await AccountService.Login(Account);
            
            if (!Account.IsValidated)
            {
                switch (Account.Errors.Values.FirstOrDefault())
                {
                    case "EmailNotExisted":
                        return BadRequest("Email không tồn tại.");
                    case "PasswordNotMatch":
                        return BadRequest("Bạn đã nhập sai mật khẩu.");
                }
            }
            Response.Cookies.Append("Token", Account.Token);
            Account_AccountDTO Account_AccountDTO = new Account_AccountDTO(Account);
            Account_AccountDTO.Token = Account.Token;
            return Ok(Account_AccountDTO);
        }
        #endregion

        [Route(AccountRoute.ChangePassword), HttpPost]
        public async Task<ActionResult<Account_AccountDTO>> ChangePassword([FromBody] Account_ChangePasswordDTO Account_ChangePasswordDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!Account_ChangePasswordDTO.Password.Equals(Account_ChangePasswordDTO.ConfirmPassword))
            {
                return BadRequest("Xác nhận mật khẩu không đúng.");
            }

            Account Account = new Account
            {
                Id = ExtractUserId(),
                Password = Account_ChangePasswordDTO.OldPassword,
                NewPassword = Account_ChangePasswordDTO.Password,
            };
            Account = await AccountService.ChangePassword(Account);
            Account_AccountDTO Account_AccountDTO = new Account_AccountDTO(Account);
            if (! Account.IsValidated)
                return BadRequest("Bạn đã nhập sai mật khẩu cũ.");
            return Ok(Account_AccountDTO);
        }

        #region Forgot Password
        [AllowAnonymous]
        [Route(AccountRoute.ForgotPassword), HttpPost]
        public async Task<ActionResult> ForgotPassword([FromBody] Account_ForgotPasswordDTO Account_ForgotPasswordDTO)
        {
            AccountFilter AccountFilter = new AccountFilter
            {
                Skip = 0,
                Take = 1,
                Email = new StringFilter { Equal = Account_ForgotPasswordDTO.Email.ToLower() },
                Selects = AccountSelect.Email
            };

            var count = await AccountService.Count(AccountFilter);
            if (count == 0)
            {
                return BadRequest("Email không tồn tại.");
            }

            Account Account = new Account { Email = Account_ForgotPasswordDTO.Email };

            Account.PasswordRecoveryCode = AccountService.GenerateCode();
            Account.ExpiredTimeCode = (DateTime.Now).AddSeconds(1800);

            Account = await AccountService.ForgotPassword(Account);

            //var Mail = new Mail();
            //Mail.Recipients = new List<string> { Account_ForgotPasswordDTO.Email };
            //Mail.Subject = "Verify Code";
            //Mail.Body = "Mã khôi phục mật khẩu của bạn là " + Account.PasswordRecoveryCode + " có hiệu lực đến " + Account.ExpiredTimeCode;
            //Thread sendMailThread = new Thread(() => MailService.Send(Mail));
            //sendMailThread.Start();

            return Ok(Account.Id);
        }

        [AllowAnonymous]
        [Route(AccountRoute.VerifyCode), HttpPost]
        public async Task<ActionResult> VerifyCode([FromBody] Account_VerifyCodeDTO Account_VerifyCodeDTO)
        {
            Account Account = await AccountService.Get(Account_VerifyCodeDTO.Id);
            if (DateTime.Now > Account.ExpiredTimeCode)
            {
                return BadRequest("Mã xác nhận đã hết hạn.");
            }
            if (!Account.PasswordRecoveryCode.Equals(Account_VerifyCodeDTO.PasswordRecoveryCode))
            {
                return BadRequest("Mã xác nhận không đúng.");
            }

            Account.PasswordRecoveryCode = Enums.Enums.VERIFIED_CODE;
            Account = await AccountService.VerifyCode(Account);

            return Ok(Account.Id);
        }
        [AllowAnonymous]
        [Route(AccountRoute.RecoveryPassword), HttpPost]
        public async Task<ActionResult> RecoveryPassword([FromBody] Account_RecoveryPasswordDTO Account_RecoveryPasswordDTO)
        {
            Account Account = await AccountService.Get(Account_RecoveryPasswordDTO.Id);
            if (!Account.PasswordRecoveryCode.Equals(Enums.Enums.VERIFIED_CODE))
            {
                return BadRequest("Bạn cần nhập mã xác nhận từ Email trước khi khôi phục mật khẩu.");
            }
            if (!Account_RecoveryPasswordDTO.Password.Equals(Account_RecoveryPasswordDTO.ConfirmPassword))
            {
                return BadRequest("Xác nhận mật khẩu không đúng.");
            }

            Account.Password = Account_RecoveryPasswordDTO.Password;
            Account = await AccountService.RecoveryPassword(Account);
            return Ok(Account.Email);
        }
        #endregion

        [Route(AccountRoute.Get), HttpPost]
        public async Task<ActionResult<Account_AccountDTO>> Get()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            var UserId = ExtractUserId();
            Account Account = await AccountService.Get(UserId);
            return new Account_AccountDTO(Account);
        }

        [AllowAnonymous]
        [Route(AccountRoute.Register), HttpPost]
        public async Task<ActionResult<Account_AccountDTO>> Register([FromBody] Account_RegisterDTO Account_RegisterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!Account_RegisterDTO.Password.Equals(Account_RegisterDTO.ConfirmPassword))
            {
                return BadRequest("Xác nhận mật khẩu không đúng.");
            }

            Account Account = new Account
            {
                Email = Account_RegisterDTO.Email,
                Phone = Account_RegisterDTO.Phone,
                Password = Account_RegisterDTO.Password
            };

            Account = await AccountService.Create(Account);
            Account_AccountDTO Account_AccountDTO = new Account_AccountDTO(Account);
            if (!Account.IsValidated)
            {
                switch (Account.Errors.Values.FirstOrDefault())
                {
                    case "EmailExisted":
                        return BadRequest("Email đã tồn tại.");
                    case "EmailInvalid":
                        return BadRequest("Email không hợp lệ.");
                    case "PhoneEmpty":
                        return BadRequest("Số điện thoại không được bỏ trống.");
                }
            }
            return Ok(Account_AccountDTO);
        }

        [Route(AccountRoute.Update), HttpPost]
        public async Task<ActionResult<Account_AccountDTO>> Update([FromBody] Account_AccountDTO Account_AccountDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Account Account = new Account
            {
                Id = Account_AccountDTO.Id,
                Phone = Account_AccountDTO.Phone,
                DisplayName = Account_AccountDTO.DisplayName,
                Address = Account_AccountDTO.Address,
                Avatar = Account_AccountDTO.Avatar,
                Dob = Account_AccountDTO.Dob
            };
            Account.Id = ExtractUserId();
            Account = await AccountService.Update(Account);
            Account_AccountDTO = new Account_AccountDTO(Account);
            if (Account.IsValidated)
                return Account_AccountDTO;
            else
                return BadRequest("Số điện thoại không được bỏ trống.");
        }

        [Route(AccountRoute.LikeFood), HttpPost]
        public async Task<ActionResult<Account_AccountDTO>> LikeFood([FromBody] Account_AccountDTO Account_AccountDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var Id = ExtractUserId();
            Account Account = new Account
            {
                Id = Id,
                AccountFoodFavorites = Account_AccountDTO.Account_AccountFoodFavorites.Select(a => new AccountFoodFavorite
                {
                    AccountId = Id,
                    FoodId = a.FoodId
                }).ToList()
            };
            Account = await AccountService.LikeFood(Account);
            Account_AccountDTO = new Account_AccountDTO(Account);
            return Ok(Account_AccountDTO);
        }

        private long ExtractUserId()
        {
            return long.TryParse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value, out long u) ? u : 0;
        }

        #region Đăng nhập bằng google 
        //private async Task<ActionResult> GoogleCallback(string code)
        //{
        //    if (code != null)
        //    {
        //        string gurl = "code=" + code + "&client_id=" + clientId +
        //                 "&client_secret=" + clientSecret + "&redirect_uri=" + redirectUri + "&grant_type=" + grant_type;
        //        var account = await POSTResultAsync(gurl);
        //        if (account != null)
        //        {
        //            //Check có tồn tại trong danh sách user hay không? 
        //            AccountDAO AccountDAO = context.Account
        //                .Where(a => a.Email.ToLower() == account.Email)
        //                .FirstOrDefault();
        //            if (AccountDAO != null)
        //            {
        //                string token = CryptographyExtentions.CreateToken(account.Id, account.Email);
        //                Response.Cookies.Append("Token", token);
        //                return Redirect("/");
        //            }
        //            return RedirectToAction("Login");
        //        }
        //    }

        //    return RedirectToAction("Login");
        //}

        //private async Task<AccountDAO> POSTResultAsync(string e)
        //{
        //    try
        //    {
        //        // variables to store parameter values
        //        string url = "https://accounts.google.com/o/oauth2/token";

        //        // creates the post data for the POST request
        //        string postData = (e);

        //        // create the POST request
        //        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
        //        webRequest.Method = "POST";
        //        webRequest.ContentType = "application/x-www-form-urlencoded";
        //        webRequest.ContentLength = postData.Length;

        //        // POST the data
        //        using (StreamWriter requestWriter2 = new StreamWriter(webRequest.GetRequestStream()))
        //        {
        //            requestWriter2.Write(postData);
        //        }

        //        //This actually does the request and gets the response back
        //        HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse();

        //        string googleAuth;

        //        using (StreamReader responseReader = new StreamReader(resp.GetResponseStream()))
        //        {
        //            //dumps the HTML from the response into a string variable
        //            googleAuth = responseReader.ReadToEnd();


        //        }

        //        gLoginInfo gli = JsonConvert.DeserializeObject<gLoginInfo>(googleAuth);

        //        // lấy thông tin của gmail
        //        GoogleJsonWebSignature.Payload validPayload = await GoogleJsonWebSignature.ValidateAsync(gli.id_token);
        //        AccountDAO account = new AccountDAO();
        //        account.Email = validPayload.Email;
        //        account.DisplayName = validPayload.Name;
        //        return account;
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return null;
        //}
        #endregion
    }

    public class gLoginClaims
    {
        public string aud, iss, email_verified, at_hash, azp, email, sub;
        public int exp, iat;
    }
    public class gLoginInfo
    {
        public string access_token, token_type, id_token;
        public int expires_in;
    }
}
