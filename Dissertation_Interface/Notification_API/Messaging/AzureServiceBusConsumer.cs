using System.Text;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;
using Notification_API.Data.Models.Dto;
using Notification_API.Services;
using Notification_API.Settings;
using Shared.Constants;
using Shared.DTO;
using Shared.Logging;
using Shared.Settings;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Notification_API.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly EmailService _emailService;
    private readonly AuditLogService _auditLogService;
    private readonly ServiceBusProcessor _emailLoggerProcessor;
    private readonly ServiceBusProcessor _auditLoggerProcessor;
    private readonly IAppLogger<AzureServiceBusConsumer> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly ServiceBusSettings _serviceBusSettings;
    private readonly SendGridSettings _sendgridSettings;

    public AzureServiceBusConsumer(
        EmailService emailService, IWebHostEnvironment env,
        AuditLogService auditLogService,
        IAppLogger<AzureServiceBusConsumer> logger,
        IOptions<ServiceBusSettings> serviceBusSettings,
        IOptions<SendGridSettings> sendgridSettings)
    {
        this._emailService = emailService;
        this._auditLogService = auditLogService;
        this._env = env;
        this._logger = logger;
        this._serviceBusSettings = serviceBusSettings.Value;
        this._sendgridSettings = sendgridSettings.Value;

        var client = new ServiceBusClient(this._serviceBusSettings.ServiceBusConnectionString);
        this._emailLoggerProcessor = client.CreateProcessor(this._serviceBusSettings.EmailLoggerQueue);
        this._auditLoggerProcessor = client.CreateProcessor(this._serviceBusSettings.AuditLoggerQueue);
    }

    #region Processor Methods
    public async Task Start()
    {
        // this is the processor for processing emails
        this._emailLoggerProcessor.ProcessMessageAsync += OnEmailRequestReceived;
        this._emailLoggerProcessor.ProcessErrorAsync += ErrorHandler;
        await this._emailLoggerProcessor.StartProcessingAsync();

        //this is the processor for processing audit logs
        this._auditLoggerProcessor.ProcessMessageAsync += OnAuditLogRequestReceived;
        this._auditLoggerProcessor.ProcessErrorAsync += ErrorHandler;
        await this._auditLoggerProcessor.StartProcessingAsync();
    }

    public async Task Stop()
    {
        await this._auditLoggerProcessor.StopProcessingAsync();
        await this._auditLoggerProcessor.DisposeAsync();

        await this._emailLoggerProcessor.StopProcessingAsync();
        await this._emailLoggerProcessor.DisposeAsync();
    }

    private async Task OnEmailRequestReceived(ProcessMessageEventArgs args)
    {
        try
        {
            // deserialize the incoming request
            ServiceBusReceivedMessage? message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            PublishEmailDto? emailDto = JsonConvert.DeserializeObject<PublishEmailDto>(body);
            var userEmail = emailDto?.User?.Email;

            //generate the email template
            if (emailDto != null && !string.IsNullOrEmpty(userEmail))
            {
                var emailBody = "";
                var emailType = "";
                var subject = "";
                switch (emailDto.EmailType)
                {
                    case EmailType.EmailTypeResetPasswordEmail:
                        emailBody = await GenerateResetPasswordEmailBody(emailDto);
                        emailType = EmailType.EmailTypeResetPasswordEmail;
                        subject = EmailSubject.EmailSubjectForResetPassword;
                        break;
                    case EmailType.EmailTypeAdminConfirmationEmail:
                        emailBody = await GenerateAdminConfirmationEmailBody(emailDto);
                        emailType = EmailType.EmailTypeAdminConfirmationEmail;
                        subject = EmailSubject.EmailSubjectForAdminEmailConfirmation;
                        break;
                    case EmailType.EmailTypeAccountActivationEmail:
                        emailBody = await GenerateAccountUnLockedEmailBody(emailDto);
                        emailType = EmailType.EmailTypeAccountActivationEmail;
                        subject = EmailSubject.EmailSubjectForAccountUnlocked;
                        break;
                    case EmailType.EmailTypeAccountDeactivationEmail:
                        emailBody = await GenerateAccountLockedEmailBody(emailDto);
                        emailType = EmailType.EmailTypeResetPasswordEmail;
                        subject = EmailSubject.EmailSubjectForAccountLockedOut;
                        break;
                }

                var logEmailDto = new LogEmailRequestDto
                {
                    Message = emailBody,
                    EmailType = emailType,
                    Email = userEmail,
                    Subject = subject
                };

                EmailResponseDto? response = await this._emailService.SaveAndSendEmail(logEmailDto);

                if (response is { Result.IsSuccessStatusCode: true, Message: not null })
                {
                    await this._emailService.UpdateEmailLogger(response.Message);
                    this._logger.LogInformation("Reset Password Email has been sent successfully for this user - {@PublishEmailDto}", emailDto);
                    await args.CompleteMessageAsync(args.Message);
                }
                else
                {
                    this._logger.LogWarning("An error occurred whilst processing the email queue  - {@EmailResponseDto}", response);
                }
            }
            else
            {
                this._logger.LogWarning("Processing Email Queue: The email DTO serialized is not null or the user email is empty");
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError("An exception occurred whilst processing the email queue - {0}", ex);

        }
    }
    private async Task OnAuditLogRequestReceived(ProcessMessageEventArgs args)
    {
        ServiceBusReceivedMessage? message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);
        AuditLogDto? auditLogDto = JsonConvert.DeserializeObject<AuditLogDto>(body);

        if (auditLogDto != null)
        {
            EmailResponseDto emailResponse = await this._auditLogService.SaveAuditLog(auditLogDto);
            if (emailResponse.IsSuccess)
            {
                this._logger.LogInformation("Audit Log saved successfully for this action {0}", auditLogDto.EventType);
                await args.CompleteMessageAsync(args.Message);
            }
            else
            {
                this._logger.LogWarning("An error occurred whilst saving the audit log for this action {0}", auditLogDto.EventType);
            }
        }
    }
    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        this._logger.LogError("An error occurred while processing messages on the queue", args.Exception.ToString());
        return Task.CompletedTask;
    }
    #endregion

    #region Generate Email Body Methods
    //this should be in the email service but the singleton registration for the services is a bottleneck
    private async Task<string> GenerateResetPasswordEmailBody(PublishEmailDto request)
    {
        var pathToFile = this._env.WebRootPath
                         + Path.DirectorySeparatorChar
                         + "Templates"
                         + Path.DirectorySeparatorChar
                         + "EmailTemplates"
                         + Path.DirectorySeparatorChar
                         + "Change_Password.html";

        var builder = new BodyBuilder();

        using StreamReader sourceReader = File.OpenText(pathToFile);
        builder.HtmlBody = await sourceReader.ReadToEndAsync();

        //{0} : Subject
        const string subject = EmailSubject.EmailSubjectForResetPassword;
        //{1} : Date
        //{2} : FirstName
        //{3} : Username
        //{4} : Callback URL

        var messageBody = string.Format(builder.HtmlBody,
            subject,
            $"{DateTime.Now:dddd, d MMMM yyyy}",
            request.User?.FirstName,
            request.User?.UserName,
            request.CallbackUrl
        );

        return messageBody;
    }
    private async Task<string> GenerateAdminConfirmationEmailBody(PublishEmailDto request)
    {
        var pathToFile = this._env.WebRootPath
                         + Path.DirectorySeparatorChar
                         + "Templates"
                         + Path.DirectorySeparatorChar
                         + "EmailTemplates"
                         + Path.DirectorySeparatorChar
                         + "Welcome_Email_Admin.html";

        var builder = new BodyBuilder();

        using StreamReader sourceReader = File.OpenText(pathToFile);
        builder.HtmlBody = await sourceReader.ReadToEndAsync();

        //{0} : Subject
        const string subject = EmailSubject.EmailSubjectForAdminEmailConfirmation;
        //{1} : Date
        //{2} : FirstName
        //{3} : AdminEmail
        var adminEmail = this._sendgridSettings.AdminEmail;
        //{4} :Callback URL

        var messageBody = string.Format(builder.HtmlBody,
            subject,
            $"{DateTime.Now:dddd, d MMMM yyyy}",
            request.User?.FirstName,
            adminEmail,
            request.CallbackUrl
        );

        return messageBody;
    }
    private async Task<string> GenerateAccountUnLockedEmailBody(PublishEmailDto request)
    {
        var pathToFile = this._env.WebRootPath
                         + Path.DirectorySeparatorChar
                         + "Templates"
                         + Path.DirectorySeparatorChar
                         + "EmailTemplates"
                         + Path.DirectorySeparatorChar
                         + "Account_Unlocked.html";

        var builder = new BodyBuilder();

        using StreamReader sourceReader = File.OpenText(pathToFile);
        builder.HtmlBody = await sourceReader.ReadToEndAsync();

        //{0} : Subject
        const string subject = EmailSubject.EmailSubjectForAccountUnlocked;
        //{1} : Date
        //{2} : FirstName

        var messageBody = string.Format(builder.HtmlBody,
            subject,
            $"{DateTime.Now:dddd, d MMMM yyyy}",
            request.User?.FirstName);
        return messageBody;
    }
    private async Task<string> GenerateAccountLockedEmailBody(PublishEmailDto request)
    {
        var pathToFile = this._env.WebRootPath
                         + Path.DirectorySeparatorChar
                         + "Templates"
                         + Path.DirectorySeparatorChar
                         + "EmailTemplates"
                         + Path.DirectorySeparatorChar
                         + "Account_Locked_Out.html";

        var builder = new BodyBuilder();

        using StreamReader sourceReader = File.OpenText(pathToFile);
        builder.HtmlBody = await sourceReader.ReadToEndAsync();

        //{0} : Subject
        const string subject = EmailSubject.EmailSubjectForAccountLockedOut;
        //{1} : Date
        //{2} : FirstName
        var adminEmail = this._sendgridSettings.AdminEmail;
        //{3} : AdminEmail

        var messageBody = string.Format(builder.HtmlBody,
            subject,
            $"{DateTime.Now:dddd, d MMMM yyyy}",
            request.User?.FirstName,
            adminEmail
        );

        return messageBody;
    }
    #endregion
}