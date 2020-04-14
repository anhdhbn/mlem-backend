using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MM.Auth.Common;
using MM.Auth.Controller.DTO;
using MM.Auth.Entities;
using MM.Auth.Models;
using MM.Auth.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MM.Auth.Controller
{
    [Route("auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly DataContext context;
        private readonly IMailService MailService;
        private string clientId = AppSettings.GoogleClientId;
        private string clientSecret = AppSettings.GoogleClientSecret;
        private string redirectUri = AppSettings.GoogleRedirectUri;
        private string grant_type = "authorization_code";
        public AuthenticationController(DataContext context, IMailService MailService)
        {
            this.context = context;
            this.MailService = MailService;
        }

        #region Register
        [AllowAnonymous]
        [Route("register"), HttpPost]
        public async Task<ActionResult> Register([FromBody] RegisterDTO RegisterDTO)
        {
            if (!Utils.IsValidEmail(RegisterDTO.Email))
                return BadRequest("Email không hợp lệ.");

            AccountDAO AccountDAO = context.Account
                .Where(a => a.Email.ToLower() == RegisterDTO.Email.ToLower())
                .FirstOrDefault();
            if (AccountDAO != null)
            {
                return BadRequest("Email đã được sử dụng.");
            }

            AccountDAO = context.Account
                .Where(a => a.Phone.ToLower() == RegisterDTO.Phone.ToLower())
                .FirstOrDefault();
            if (AccountDAO != null)
            {
                return BadRequest("Số điện thoại đã được sử dụng.");
            }

            if (!RegisterDTO.Password.Equals(RegisterDTO.ConfirmPassword))
            {
                return BadRequest("Xác nhận mật khẩu không đúng.");
            }

            try
            {
                AccountDAO = new AccountDAO();
                AccountDAO.Email = RegisterDTO.Email;
                AccountDAO.Phone = RegisterDTO.Phone;
                AccountDAO.DisplayName = RegisterDTO.Email.Split("@").FirstOrDefault();
                AccountDAO.Salt = Convert.ToBase64String(CryptographyExtentions.GenerateSalt());
                AccountDAO.Password = RegisterDTO.Password.HashHMACSHA256(AccountDAO.Salt);
                AccountDAO.RoleId = Enums.Enums.MEMBER.GetHashCode();
                AccountDAO.StatusId = Enums.Enums.STATUS_ACTIVE.GetHashCode();
                AccountDAO.CreatedAt = DateTime.Now;
                AccountDAO.UpdatedAt = DateTime.Now;

                await context.Account.AddAsync(AccountDAO);
                await context.SaveChangesAsync();

                string token = CryptographyExtentions.CreateToken(AccountDAO.Id, AccountDAO.Email);
                Response.Cookies.Append("Token", token);
                return Ok(token);
            }
            catch (Exception)
            {
                return BadRequest("Đăng ký tài khoản không thành công");
            }
            
        }
        #endregion

        #region Login
        [AllowAnonymous]
        [Route("login"), HttpPost]
        public async Task<ActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            AccountDAO AccountDAO = context.Account
                .Where(a => a.Email.ToLower() == loginDTO.Email.ToLower())
                .FirstOrDefault();
            if (AccountDAO == null)
            {
                return BadRequest("Email không tồn tại.");
            }
            bool verified = CryptographyExtentions.VerifyPassword(AccountDAO.Password, loginDTO.Password, AccountDAO.Salt);
            if (!verified)
            {
                return BadRequest("Bạn đã nhập sai mật khẩu.");
            }

            string token = CryptographyExtentions.CreateToken(AccountDAO.Id, AccountDAO.Email);
            Response.Cookies.Append("Token", token);
            return Ok(token);
        }
        #endregion

        #region Forgot Password
        [AllowAnonymous]
        [Route("forgot-password"), HttpPost]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDTO)
        {
            AccountDAO AccountDAO = context.Account
                .Where(a => a.Email.ToLower() == forgotPasswordDTO.Email.ToLower())
                .FirstOrDefault();
            if (AccountDAO == null)
            {
                return BadRequest("Tài khoản không tồn tại.");
            }
            AccountDAO.PasswordRecoveryCode = CryptographyExtentions.GenerateCode();
            AccountDAO.ExpiredTimeCode = (DateTime.Now).AddSeconds(180);

            await context.Account.Where(a => a.Email.ToLower() == forgotPasswordDTO.Email.ToLower()).UpdateFromQueryAsync(a => new AccountDAO
            {
                PasswordRecoveryCode = AccountDAO.PasswordRecoveryCode,
                ExpiredTimeCode = AccountDAO.ExpiredTimeCode
            });

            var Mail = new Mail();
            Mail.Recipients = new List<string> { forgotPasswordDTO.Email };
            Mail.Subject = "Verify Code";
            Mail.Body = "Mã khôi phục mật khẩu của bạn là " + AccountDAO.PasswordRecoveryCode + " có hiệu lực đến " + AccountDAO.ExpiredTimeCode;
            Thread sendMailThread = new Thread(() => MailService.Send(Mail));
            sendMailThread.Start();

            return Ok(AccountDAO.Email);
        }

        [AllowAnonymous]
        [Route("verify-code"), HttpPost]
        public async Task<ActionResult> VerifyCode([FromBody] ForgotPasswordDTO forgotPasswordDTO)
        {
            AccountDAO AccountDAO = context.Account
                .Where(a => a.Email.ToLower() == forgotPasswordDTO.Email.ToLower())
                .FirstOrDefault();
            if(DateTime.Now > AccountDAO.ExpiredTimeCode)
            {
                return BadRequest("Mã xác nhận đã hết hạn.");
            }
            if (!AccountDAO.PasswordRecoveryCode.Equals(forgotPasswordDTO.PasswordRecoveryCode))
            {
                return BadRequest("Mã xác nhận không đúng.");
            }

            await context.Account.Where(a => a.Email.ToLower() == forgotPasswordDTO.Email.ToLower()).UpdateFromQueryAsync(a => new AccountDAO
            {
                PasswordRecoveryCode = Enums.Enums.VERIFIED_CODE
            });

            return Ok(AccountDAO.Email);
        }
        [AllowAnonymous]
        [Route("recovery-password"), HttpPost]
        public async Task<ActionResult> RecoveryPassword([FromBody] RecoveryPasswordDTO recoveryPasswordDTO)
        {
            AccountDAO AccountDAO = context.Account
                .Where(a => a.Email.ToLower() == recoveryPasswordDTO.Email.ToLower())
                .FirstOrDefault();
            if (!AccountDAO.PasswordRecoveryCode.Equals(Enums.Enums.VERIFIED_CODE))
            {
                return BadRequest("Bạn cần nhập mã xác nhận từ Email trước khi khôi phục mật khẩu.");
            }
            if (!recoveryPasswordDTO.Password.Equals(recoveryPasswordDTO.ConfirmPassword))
            {
                return BadRequest("Xác nhận mật khẩu không đúng.");
            }

            var Salt = Convert.ToBase64String(CryptographyExtentions.GenerateSalt());
            await context.Account.Where(a => a.Email.ToLower() == recoveryPasswordDTO.Email.ToLower()).UpdateFromQueryAsync(a => new AccountDAO
            {
                Password = recoveryPasswordDTO.Password.HashHMACSHA256(Salt),
                Salt = Salt
            });
            return Ok(AccountDAO.Email);
        }
        #endregion

        //#region Profile
        //[Route("get-profile")]
        //public async Task<ActionResult<AccountDTO>> GetProfile(AccountDTO AccountDTO)
        //{

        //}
        //#endregion

        //#region Change password
        //[Route("change-password"), HttpPost]
        //public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        //{
        //    AccountDAO AccountDAO = context.Account
        //        .Where(a => a.Email.ToLower() == changePasswordDTO.Email.ToLower())
        //        .FirstOrDefault();

        //    bool verified = CryptographyExtentions.VerifyPassword(AccountDAO.Password, changePasswordDTO.OldPassword, AccountDAO.Salt);
        //    if (!verified)
        //    {
        //        return BadRequest("Mật khẩu cũ không đúng.");
        //    }

        //    if (!changePasswordDTO.Password.Equals(changePasswordDTO.ConfirmPassword))
        //    {
        //        return BadRequest("Xác nhận mật khẩu không đúng.");
        //    }

        //    var Salt = Convert.ToBase64String(CryptographyExtentions.GenerateSalt());
        //    await context.Account.Where(a => a.Email.ToLower() == changePasswordDTO.Email.ToLower()).UpdateFromQueryAsync(a => new AccountDAO
        //    {
        //        Password = changePasswordDTO.Password.HashHMACSHA256(Salt),
        //        Salt = Salt
        //    });
        //    return Ok(AccountDAO.Email);
        //}
        //#endregion

        #region Đăng nhập bằng google 
        private async Task<ActionResult> GoogleCallback(string code)
        {
            if (code != null)
            {
                string gurl = "code=" + code + "&client_id=" + clientId +
                         "&client_secret=" + clientSecret + "&redirect_uri=" + redirectUri + "&grant_type=" + grant_type;
                var account = await POSTResultAsync(gurl);
                if (account != null)
                {
                    //Check có tồn tại trong danh sách user hay không? 
                    AccountDAO AccountDAO = context.Account
                        .Where(a => a.Email.ToLower() == account.Email)
                        .FirstOrDefault();
                    if (AccountDAO != null)
                    {
                        string token = CryptographyExtentions.CreateToken(account.Id, account.Email);
                        Response.Cookies.Append("Token", token);
                        return Redirect("/");
                    }
                    return RedirectToAction("Login");
                }
            }

            return RedirectToAction("Login");
        }

        private async Task<AccountDAO> POSTResultAsync(string e)
        {
            try
            {
                // variables to store parameter values
                string url = "https://accounts.google.com/o/oauth2/token";

                // creates the post data for the POST request
                string postData = (e);

                // create the POST request
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContentLength = postData.Length;

                // POST the data
                using (StreamWriter requestWriter2 = new StreamWriter(webRequest.GetRequestStream()))
                {
                    requestWriter2.Write(postData);
                }

                //This actually does the request and gets the response back
                HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse();

                string googleAuth;

                using (StreamReader responseReader = new StreamReader(resp.GetResponseStream()))
                {
                    //dumps the HTML from the response into a string variable
                    googleAuth = responseReader.ReadToEnd();


                }

                gLoginInfo gli = JsonConvert.DeserializeObject<gLoginInfo>(googleAuth);

                // lấy thông tin của gmail
                GoogleJsonWebSignature.Payload validPayload = await GoogleJsonWebSignature.ValidateAsync(gli.id_token);
                AccountDAO account = new AccountDAO();
                account.Email = validPayload.Email;
                account.DisplayName = validPayload.Name;
                return account;
            }
            catch (Exception ex)
            {

            }
            return null;
        }
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
