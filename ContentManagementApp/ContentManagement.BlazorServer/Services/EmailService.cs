using ContentManagement.BlazorServer.Data;
using ContentManagement.BlazorServer.Options;
using ContentManagement.BlazorServer.Services.Contracts;
using ContentManagement.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;


namespace ContentManagement.BlazorServer.Services
{
    public class EmailService : IEmailService, IEmailSender<ApplicationUser>
    {
        private readonly EmailConfiguration _emailConfiguration;

        public EmailService(IOptions<EmailConfiguration> emailConfiguration)
        {
            _emailConfiguration = emailConfiguration.Value;
        }

        public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
        {
            string subject = "Confirm your email";
            string body = $"Please confirm your account by <a href='{confirmationLink}'>clicking here.</a>";
            await SendEmailAsync(email, subject, body);
        }

        public async Task SendEmailAsync(EmailModel emailRequest)
        {
            var email = new MimeMessage();
            var builder = new BodyBuilder();

            email.From.Add(MailboxAddress.Parse(_emailConfiguration.Username));
            email.To.Add(MailboxAddress.Parse(emailRequest.To));
            email.Subject = emailRequest.Subject;
            builder.HtmlBody = string.Format(emailRequest.Body);
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailConfiguration.Host, _emailConfiguration.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailConfiguration.Username, _emailConfiguration.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            var builder = new BodyBuilder();

            message.From.Add(MailboxAddress.Parse(_emailConfiguration.Username));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;
            builder.HtmlBody = string.Format(htmlMessage);
            message.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailConfiguration.Host, _emailConfiguration.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailConfiguration.Username, _emailConfiguration.Password);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
        {
            throw new NotImplementedException();
        }

        public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
        {
            string subject = "Confirm your email";
            string body = $"Please reset your password by <a href='{resetLink}'>clicking here.</a>";
            await SendEmailAsync(email, subject, body);
        }
    }
}
