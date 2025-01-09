using CommunityManager.Models;
using System.Net;
using System.Net.Mail;

namespace CommunityManager.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task Send(EmailMetaData emailMetaData)
        {
            var email = configuration.GetValue<string>("EmailConfiguration:Email");
            var host = configuration.GetValue<string>("EmailConfiguration:Host");
            int port = configuration.GetValue<int>("EmailConfiguration:Port");
            var password = configuration.GetValue<string>("EmailConfiguration:Password");

            var smtpClient = new SmtpClient(host, port);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;

            smtpClient.Credentials = new NetworkCredential(email, password);
            smtpClient.Timeout = 1;

            var message = new MailMessage(email!, emailMetaData.ToAddress, emailMetaData.Subject, emailMetaData.Body);
            message.IsBodyHtml = true;
            await smtpClient.SendMailAsync(message);
        }
    }
}
