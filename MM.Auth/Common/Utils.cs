using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MM.Auth.Common
{
    public static class Utils
    {
        public static bool IsValidEmail(string email)
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

        //public static async Task RegisterMail(User user)
        //{
        //    if (string.IsNullOrEmpty(user.Email)) return;
        //    string SendEmail = "12finalteam@gmail.com";
        //    string SendEmailPassword = "TF123456a@";

        //    var loginInfo = new NetworkCredential(SendEmail, SendEmailPassword);
        //    var msg = new MailMessage();
        //    var smtpClient = new SmtpClient("smtp.gmail.com", 587);

        //    string body = "Tài khoản của bạn đã được đăng ký!\n";
        //    body += "Id: " + user.Username + "\n";
        //    body += "Password: " + user.Password;
        //    try
        //    {
        //        msg.From = new MailAddress(SendEmail);
        //        msg.To.Add(new MailAddress(user.Email));
        //        msg.Subject = "Đăng ký tài khoản TwelveFinal!";
        //        msg.Body = body;
        //        msg.IsBodyHtml = true;

        //        smtpClient.EnableSsl = true;
        //        smtpClient.UseDefaultCredentials = false;
        //        smtpClient.Credentials = loginInfo;
        //        await smtpClient.SendMailAsync(msg);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new BadRequestException("Error!");
        //    }
        //}

        //public static async Task RecoveryPasswordMail(User user)
        //{
        //    if (string.IsNullOrEmpty(user.Email)) return;
        //    string SendEmail = "12finalteam@gmail.com";
        //    string SendEmailPassword = "TF123456a@";

        //    var loginInfo = new NetworkCredential(SendEmail, SendEmailPassword);
        //    var msg = new MailMessage();
        //    var smtpClient = new SmtpClient("smtp.gmail.com", 587);

        //    string body = "Mật khẩu của bạn đã được khôi phục!\n";
        //    body += "Password: " + user.Password;
        //    try
        //    {
        //        msg.From = new MailAddress(SendEmail);
        //        msg.To.Add(new MailAddress(user.Email));
        //        msg.Subject = "Khôi phục mật khẩu Twelve Final!";
        //        msg.Body = body;
        //        msg.IsBodyHtml = true;

        //        smtpClient.EnableSsl = true;
        //        smtpClient.UseDefaultCredentials = false;
        //        smtpClient.Credentials = loginInfo;
        //        await smtpClient.SendMailAsync(msg);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new BadRequestException("Error!");
        //    }
        //}

        public static string GeneratePassword()
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        public static Guid CreateGuid(string name)
        {
            MD5 md5 = MD5.Create();
            Byte[] myStringBytes = ASCIIEncoding.Default.GetBytes(name);
            Byte[] hash = md5.ComputeHash(myStringBytes);
            return new Guid(hash);
        }
    }
}
