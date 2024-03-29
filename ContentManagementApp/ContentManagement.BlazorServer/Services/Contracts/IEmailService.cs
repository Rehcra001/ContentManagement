﻿using ContentManagement.Models;

namespace ContentManagement.BlazorServer.Services.Contracts
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailModel email);
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
