using ContentManagement.BlazorServer.Options;
using ContentManagement.BlazorServer.Services.Contracts;
using ContentManagement.Models;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;


namespace ContentManagement.BlazorServer.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfiguration;

        public EmailService(IOptions<EmailConfiguration> emailConfiguration)
        {
            _emailConfiguration = emailConfiguration.Value;
        }

        public async Task SendEmailAsync(EmailModel emailRequest)
        {
            var email = new MimeMessage();
            var builder = new BodyBuilder();

            email.From.Add(MailboxAddress.Parse(_emailConfiguration.Username));
            email.To.Add(MailboxAddress.Parse(emailRequest.To));
            email.Subject = emailRequest.Subject;

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailConfiguration.Host, _emailConfiguration.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailConfiguration.Username, _emailConfiguration.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
