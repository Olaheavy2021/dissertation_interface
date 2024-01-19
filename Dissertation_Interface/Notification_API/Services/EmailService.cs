using System.Net;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Notification_API.Data;
using Notification_API.Data.Models;
using Notification_API.Data.Models.Dto;
using Notification_API.Services.IServices;
using SendGrid;
using SendGrid.Helpers.Mail;
using Shared.Constants;
using Shared.Helpers;
using Shared.Logging;

namespace Notification_API.Services;

public class EmailService : IEmailService
{
    private readonly DbContextOptions<NotificationDbContext> _dbOptions;
    private readonly IConfiguration _configuration;

    public EmailService(DbContextOptions<NotificationDbContext> dbOptions, IConfiguration configuration)
    {
        this._dbOptions = dbOptions;
        this._configuration = configuration;
    }

    public async Task<string> SaveEmail(LogEmailRequestDto request)
    {
        EmailLogger emailLog = new()
        {
            Email = request.Email,
            Message = request.Message,
            EmailType = request.EmailType,
            Status = EmailStatus.EmailStatusPending,
            EmailLogged = DateTime.Now,
            EmailIdentifier = GenerateUniqueGuid.Generate().ToString()
        };

        await using var db = new NotificationDbContext(this._dbOptions);
        await db.EmailLoggers.AddAsync(emailLog);
        if (await db.SaveChangesAsync() != 0)
        {
            return emailLog.EmailIdentifier;
        }

        return string.Empty;
    }

    public async Task<EmailResponseDto?> SaveAndSendEmail(LogEmailRequestDto request)
    {
        var response = new EmailResponseDto { IsSuccess = false, Message = ErrorMessages.EmailServiceUnableToSaveEmail };
        //TODO:Remove this for production
        var testEmail = this._configuration.GetValue<string>("SendGridSettings:TestEmail");

        //save email into the database
        var emailIdentifier = await SaveEmail(request);
        if (!string.IsNullOrEmpty(emailIdentifier))
        {
            var apiKey = this._configuration.GetValue<string>("SendGridSettings:ApiKey");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(this._configuration.GetValue<string>("SendGridSettings:From"), this._configuration.GetValue<string>("SendGridSettings:Name"));
            //TODO:Remove this for production
            //var to = new EmailAddress(request.Email);
            var to = new EmailAddress(testEmail);
            var subject = request.Subject;
            SendGridMessage? msg = MailHelper.CreateSingleEmail(from, to, subject, null, request.Message);
            Response? sendGridResponse = await client.SendEmailAsync(msg);

            response.Message = emailIdentifier;
            response.IsSuccess = true;
            response.Result = sendGridResponse;
            return response;
        }

        return response;
    }

    public async Task UpdateEmailLogger(string emailIdentifier)
    {
        await using var db = new NotificationDbContext(this._dbOptions);
        EmailLogger? email = await db.EmailLoggers.FirstOrDefaultAsync(e => e.EmailIdentifier == emailIdentifier);
        if (email != null)
        {
            email.EmailSent = DateTime.Now;
            email.Status = EmailStatus.EmailStatusSent;
            db.EmailLoggers.Update(email);
            await db.SaveChangesAsync();
        }
    }

    private async Task SaveToFile(string name, string content, string webrootPath)
    {
        var fullPath = Path.Combine(webrootPath, name);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? string.Empty);
        await File.WriteAllTextAsync(fullPath, content);
    }
}