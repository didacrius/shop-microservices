using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Ordering.Infrastructure.Mail;

public class EmailService : IEmailService
{
    public EmailSettings Settings { get; }
    public ILogger<EmailService> Logger { get; }

    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
    {
        Settings = settings.Value ?? throw new ArgumentNullException(nameof(settings.Value));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Send(Email email)
    {
        var client = new SendGridClient(Settings.ApiKey);

        var subject = email.Subject;
        var to = new EmailAddress(email.To);
        var body = email.Body;

        var from = new EmailAddress
        {
            Email = Settings.FromAddress,
            Name = Settings.FromName
        };

        var sendGridMessage = MailHelper.CreateSingleEmail(from, to, subject, body, body);
        var response = await client.SendEmailAsync(sendGridMessage);
        
        if (response.IsSuccessStatusCode)
            Logger.LogInformation("Email sent");
        else
            Logger.LogInformation("Email sending error");

        return response.IsSuccessStatusCode;
    }
}