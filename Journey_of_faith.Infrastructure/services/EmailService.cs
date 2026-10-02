using Journey_of_faith.Application.common.interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Journey_of_faith.Infrastructure.services;

public class EmailService : IEmailService
{
    private readonly EmailSendProperties emailSendProperties;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _logger = logger;
        emailSendProperties = config.GetSection(EmailSendPropertiesExtensions.COLLECTION_EMAIL)
            .Get<EmailSendProperties>() ?? new EmailSendProperties();
    }
    public async Task<string> SendEmailAsync(string email, string subject, string body)
    {
        try
        {
            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress(emailSendProperties.SenderName ?? "System", emailSendProperties.Email));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = body
            };

            using(var client = new SmtpClient())
            {
                await client.ConnectAsync(
                    emailSendProperties.Host, 
                    emailSendProperties.Port,
                    MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(
                    emailSendProperties.Email,
                    emailSendProperties.KeySecret);

                await client.SendAsync(message);

                await client.DisconnectAsync(true);
            }
            _logger.LogInformation("Email sent successfully");
            return "Gửi email thành công.";
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to send email");
            return $"Gửi email thất bại: {exception.Message}";
        }
    }
}


public class EmailSendProperties
{
    public string Email {get; set;}
    public string KeySecret {get; set;}
    public string Host {get; set;}
    public int Port {get; set;}

    public string SenderName {get; set;}
}

public static class EmailSendPropertiesExtensions
{
    public static readonly string COLLECTION_EMAIL= "SettingsEmailSender";
}
