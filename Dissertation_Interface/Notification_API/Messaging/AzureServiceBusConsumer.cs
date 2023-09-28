using System.Text;
using Azure.Messaging.ServiceBus;
using MimeKit;
using Newtonsoft.Json;
using Notification_API.Data.Models.Dto;
using Notification_API.Services;
using Notification_API.Services.IServices;
using Shared.Constants;
using Shared.Logging;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Notification_API.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly string _serviceBusConnectionString;
    private readonly string _registerAdminUserQueue;
    private readonly string _resetPasswordQueue;
    private readonly IConfiguration _configuration;
    private readonly EmailService _emailService;
    private readonly ServiceBusProcessor _registerAdminUserProcessor;
    private readonly ServiceBusProcessor _resetPasswordProcessor;
    private readonly IAppLogger<AzureServiceBusConsumer> _logger;
    private readonly IWebHostEnvironment _env;

    public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService, IWebHostEnvironment env, IAppLogger<AzureServiceBusConsumer> logger)
    {
        this._configuration = configuration;
        this._emailService = emailService;
        this._configuration = configuration;
        this._env = env;
        this._logger = logger;

        this._serviceBusConnectionString = this._configuration.GetValue<string>("ServiceBusConnectionString") ?? string.Empty;
        this._registerAdminUserQueue = this._configuration.GetValue<string>("TopicAndQueueNames:RegisterAdminUserQueue") ?? string.Empty;
        this._resetPasswordQueue = this._configuration.GetValue<string>("TopicAndQueueNames:ResetPasswordQueue") ?? string.Empty;

        var client = new ServiceBusClient(this._serviceBusConnectionString);
        this._registerAdminUserProcessor = client.CreateProcessor(this._registerAdminUserQueue);
        this._resetPasswordProcessor = client.CreateProcessor(this._resetPasswordQueue);
    }

    public async Task Start()
    {
        this._registerAdminUserProcessor.ProcessMessageAsync += OnAdminUserRegisterRequestReceived;
        this._registerAdminUserProcessor.ProcessErrorAsync += ErrorHandler;
        await this._registerAdminUserProcessor.StartProcessingAsync();

        this._resetPasswordProcessor.ProcessMessageAsync += OnResetPasswordRequestReceived;
        this._resetPasswordProcessor.ProcessErrorAsync += ErrorHandler;
        await this._resetPasswordProcessor.StartProcessingAsync();
    }

    public async Task Stop()
    {
        await this._registerAdminUserProcessor.StopProcessingAsync();
        await this._registerAdminUserProcessor.DisposeAsync();
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
            const string subject = "Dissertation Interface - Reset Password";
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
                ResponseDto response = await this._emailService.ResetPasswordEmailAndLog(messageBody,
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
                const string subject = EmailSubject.EmailSubjectEmailConfirmation;
                //{1} : Date
                //{2} : FirstName
                //{3} : Email
                //{4} : Username
                //{5} : Password
                //{6} : Callback URL

                var messageBody = string.Format(builder.HtmlBody,
                    subject,
                    $"{DateTime.Now:dddd, d MMMM yyyy}",
                    emailDto?.User?.FirstName,
                    emailDto?.User?.Email,
                    emailDto?.User?.UserName,
                    SystemDefault.DefaultPassword,
                    emailDto?.CallbackUrl
                );

                // try to send email
                if (emailDto != null)
                {
                    ResponseDto response = await this._emailService.RegisterAdminUserEmailAndLog(messageBody,
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
        }
        catch (Exception ex)
        {
            this._logger.LogError("An error occurred whilst processing admin user email confirmation - {0}", ex);
        }

    }

    private static Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }
}