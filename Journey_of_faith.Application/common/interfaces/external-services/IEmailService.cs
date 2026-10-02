using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Application.common.interfaces
{
    public interface IEmailService
    {
        Task<string> SendEmailAsync(string toEmail, string subject, string body);
    }
}
