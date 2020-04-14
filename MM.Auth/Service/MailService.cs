using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MM.Auth.Common;
using MM.Auth.Config;
using MM.Auth.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MM.Auth.Service
{
    public interface IMailService : IServiceScoped
    {
        Task Send(Mail mail);
    }
    public class MailService : IMailService
    {
        private readonly EmailConfig emailConfig;
        public MailService(EmailConfig _emailConfig)
        {
            emailConfig = _emailConfig;
        }

        public async Task Send(Mail mail)
        {
            var mailMessage = CreateMail(mail);
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(emailConfig.SmtpServer, emailConfig.Port, SecureSocketOptions.StartTls);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(emailConfig.UserName, emailConfig.Password);
                    await client.SendAsync(mailMessage);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }

        private MimeMessage CreateMail(Mail mail)
        {
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress(emailConfig.From));
            foreach (string recipient in mail.Recipients)
            {
                mailMessage.To.Add(new MailboxAddress(recipient));
            }
            mailMessage.Subject = mail.Subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = mail.Body };

            mailMessage.Body = bodyBuilder.ToMessageBody();
            return mailMessage;
        }
    }
}
