using System.Text;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;
using Notification_API.Data.Models.Dto;
using Notification_API.Services;
using Notification_API.Settings;
using Shared.Constants;
using Shared.Logging;
using Shared.Settings;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Notification_API.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly EmailService _emailService;
    private readonly ServiceBusProcessor _registerAdminUserProcessor;
    private readonly ServiceBusProcessor _resetPasswordProcessor;
    private readonly ServiceBusProcessor _accountLockedOrUnlockedProcessor;
    private readonly IAppLogger<AzureServiceBusConsumer> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly ServiceBusSettings _serviceBusSettings;
    private readonly SendGridSettings _sendgridSettings;

    public AzureServiceBusConsumer(
        EmailService emailService, IWebHostEnvironment env,
        IAppLogger<AzureServiceBusConsumer> logger,
        IOptions<ServiceBusSettings> serviceBusSettings,
        IOptions<SendGridSettings> sendgridSettings)
    {
        this._emailService = emailService;
        this._env = env;
        this._logger = logger;
        this._serviceBusSettings = serviceBusSettings.Value;
        this._sendgridSettings = sendgridSettings.Value;

        var client = new ServiceBusClient(this._serviceBusSettings.ServiceBusConnectionString);
        this._registerAdminUserProcessor = client.CreateProcessor(this._serviceBusSettings.RegisterAdminUserQueue);
        this._resetPasswordProcessor = client.CreateProcessor(this._serviceBusSettings.ResetPasswordQueue);
        this._accountLockedOrUnlockedProcessor = client.CreateProcessor(this._serviceBusSettings.AccountLockedOutQueue);
    }

    public async Task Start()
    {
        //this is the processor for admin user confirmation emails
        this._registerAdminUserProcessor.ProcessMessageAsync += OnAdminUserRegisterRequestReceived;
        this._registerAdminUserProcessor.ProcessErrorAsync += ErrorHandler;
        await this._registerAdminUserProcessor.StartProcessingAsync();

        //this is the processor for reset password emails
        this._resetPasswordProcessor.ProcessMessageAsync += OnResetPasswordRequestReceived;
        this._resetPasswordProcessor.ProcessErrorAsync += ErrorHandler;
        await this._resetPasswordProcessor.StartProcessingAsync();

        //this is the processor for account activation or deactivation emails
        this._accountLockedOrUnlockedProcessor.ProcessMessageAsync += OnAccountLockedOrUnlockedRequestReceived;
        this._accountLockedOrUnlockedProcessor.ProcessErrorAsync += ErrorHandler;
        await this._accountLockedOrUnlockedProcessor.StartProcessingAsync();

    }

    public async Task Stop()
    {
        await this._registerAdminUserProcessor.StopProcessingAsync();
        await this._registerAdminUserProcessor.DisposeAsync();

        await this._resetPasswordProcessor.StopProcessingAsync();
        await this._resetPasswordProcessor.DisposeAsync();

        await this._accountLockedOrUnlockedProcessor.StopProcessingAsync();
        await this._accountLockedOrUnlockedProcessor.DisposeAsync();
    }

    private async Task OnResetPasswordRequestReceived(ProcessMessageEventArgs args)
    {
        try
        {
            ServiceBusReceivedMessage? message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            PublishEmailDto? emailDto = JsonConvert.DeserializeObject<PublishEmailDto>(body);

            // try to send email
            //Get TemplateFile located at wwwroot/Templates/EmailTemplates/
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
                emailDto?.User?.FirstName,
                emailDto?.User?.UserName,
                emailDto?.CallbackUrl
            );

            // try to send email
            if (emailDto != null)
            {
                ResponseDto? response = await this._emailService.ResetPasswordEmailAndLog(messageBody,
                    emailDto.User?.Email ?? string.Empty);
                if (response is { Result: { IsSuccessStatusCode: true }, Message: { } })
                {
                    await this._emailService.UpdateEmailLogger(response.Message);
                    this._logger.LogInformation("Reset Password Email has been sent successfully for this user - {@PublishEmailDto}", emailDto);
                    await args.CompleteMessageAsync(args.Message);
                }
                else
                {
                    this._logger.LogWarning("An error occurred whilst sending email to reset password for this user - {@ResponseDto}", response);
                }

            }
        }
        catch (Exception ex)
        {
            this._logger.LogError("An error occurred whilst sending the reset password email - {0}", ex);
        }

    }

    private async Task OnAdminUserRegisterRequestReceived(ProcessMessageEventArgs args)
    {
        try
        {
            ServiceBusReceivedMessage? message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            PublishEmailDto? emailDto = JsonConvert.DeserializeObject<PublishEmailDto>(body);
            if (emailDto != null)
            {
                this._logger.LogInformation("Processing Email Confirmation for this admin user - {@PublishEmailDto}",
                    emailDto);

                //Get TemplateFile located at wwwroot/Templates/EmailTemplates/
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
                    emailDto.User?.FirstName,
                    adminEmail,
                    emailDto.CallbackUrl
                );

                // try to send email
                ResponseDto? response = await this._emailService.RegisterAdminUserEmailAndLog(messageBody,
                     emailDto.User?.Email ?? string.Empty);
                if (response is { Result: { IsSuccessStatusCode: true }, Message: { } })
                {
                    await this._emailService.UpdateEmailLogger(response.Message);
                    this._logger.LogInformation("Email Confirmation has been sent successfully for this user - {@PublishEmailDto}", emailDto);
                    await args.CompleteMessageAsync(args.Message);
                }
                else
                {
                    this._logger.LogWarning("An error occurred whilst sending email confirmation for this user - {@ResponseDto}", response);
                }

            }
        }
        catch (Exception ex)
        {
            this._logger.LogError("An error occurred whilst processing admin user email confirmation - {0}", ex);
        }
    }

    private async Task OnAccountLockedOrUnlockedRequestReceived(ProcessMessageEventArgs args)
    {
        ServiceBusReceivedMessage? message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);
        PublishEmailDto? emailDto = JsonConvert.DeserializeObject<PublishEmailDto>(body);
        if (emailDto != null)
        {
            if (emailDto.EmailType.Equals(EmailType.EmailTypeAccountActivationEmail))
            {
                //Get TemplateFile located at wwwroot/Templates/EmailTemplates/
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
                emailDto.User?.FirstName);

                // try to send email
                ResponseDto? response = await this._emailService.AccountUnLockedEmailAndLog(messageBody,
                    emailDto.User?.Email ?? string.Empty);
                if (response is { Result: { IsSuccessStatusCode: true }, Message: { } })
                {
                    await this._emailService.UpdateEmailLogger(response.Message);
                    this._logger.LogInformation("Account unlocked email has been sent successfully for this user - {@PublishEmailDto}", emailDto);
                    await args.CompleteMessageAsync(args.Message);
                }
                else
                {
                    this._logger.LogWarning("An error occurred whilst sending account unlocked user - {@ResponseDto}", response);
                }
            }
            else
            {
                //Get TemplateFile located at wwwroot/Templates/EmailTemplates/
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
                    emailDto?.User?.FirstName,
                    adminEmail
                    );

                // try to send email
                if (emailDto != null)
                {
                    ResponseDto? response = await this._emailService.AccountLockedEmailAndLog(messageBody,
                        emailDto.User?.Email ?? string.Empty);
                    if (response is { Result: { IsSuccessStatusCode: true }, Message: { } })
                    {
                        await this._emailService.UpdateEmailLogger(response.Message);
                        this._logger.LogInformation("Account locked out email has been sent successfully for this user - {@PublishEmailDto}", emailDto);
                        await args.CompleteMessageAsync(args.Message);
                    }
                    else
                    {
                        this._logger.LogWarning("An error occurred whilst sending account locked out email - {@ResponseDto}", response);
                    }
                }
            }


        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        this._logger.LogError("An error occurred while processing messages on the queue", args.Exception.ToString());
        return Task.CompletedTask;
    }
}