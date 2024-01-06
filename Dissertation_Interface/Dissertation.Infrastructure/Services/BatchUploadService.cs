using System.Text;
using Azure.Messaging.ServiceBus;
using Dissertation.Infrastructure.Context;
using Dissertation.Infrastructure.DTO;
using Dissertation.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Shared.Constants;
using Shared.DTO;
using Shared.Helpers;
using Shared.Settings;

namespace Dissertation.Infrastructure.Services;

public class BatchUploadService : IBatchUploadService
{
    private readonly DbContextOptions<DissertationDbContext> _dbOptions;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BatchUploadService(DbContextOptions<DissertationDbContext> dbOptions, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        this._dbOptions = dbOptions;
        this._configuration = configuration;
        this._httpContextAccessor = httpContextAccessor;
    }

    public async Task ProcessSupervisorInvites(BulkUserUploadRequest bulkUserUploadRequest)
    {
        ServiceBusSettings? serviceBusSettings = this._configuration.GetSection("ServiceBusSettings").Get<ServiceBusSettings>();
        if (serviceBusSettings == null)
        {
            throw new InvalidOperationException("ServiceBusSettings must be configured in app settings.");
        }

        await using var client = new ServiceBusClient(serviceBusSettings.ServiceBusConnectionString);
        ServiceBusSender sender = client.CreateSender(serviceBusSettings.BatchUploadQueue);

        foreach (UserUploadRequest request in bulkUserUploadRequest.Requests)
        {
            var invitationCode = InviteCodeGenerator.GenerateCode(8);
            var supervisionInvite = Domain.Entities.SupervisorInvite.Create(
                StringHelpers.CapitalizeFirstLetterAndLowercaseRest(request.LastName),
                StringHelpers.CapitalizeFirstLetterAndLowercaseRest(request.FirstName),
                request.Username.ToLower(),
                request.Email.ToLower(),
                invitationCode
            );

            await using var db = new DissertationDbContext(this._dbOptions, this._httpContextAccessor);
            await db.SupervisorInvites.AddAsync(supervisionInvite);
            db.SaveChanges();

            //publish email
            await PublishSupervisionInviteMessage(request, invitationCode, sender, client);
        }
    }

    public async Task ProcessStudentInvites(BulkUserUploadRequest bulkUserUploadRequest)
    {
        ServiceBusSettings? serviceBusSettings = this._configuration.GetSection("ServiceBusSettings").Get<ServiceBusSettings>();
        if (serviceBusSettings == null)
        {
            throw new InvalidOperationException("ServiceBusSettings must be configured in app settings.");
        }

        await using var client = new ServiceBusClient(serviceBusSettings.ServiceBusConnectionString);
        ServiceBusSender sender = client.CreateSender(serviceBusSettings.BatchUploadQueue);



        foreach (UserUploadRequest request  in bulkUserUploadRequest.Requests)
        {
            var invitationCode = InviteCodeGenerator.GenerateCode(8);
            var studentInvite = Domain.Entities.StudentInvite.Create(
                StringHelpers.CapitalizeFirstLetterAndLowercaseRest(request.LastName),
                StringHelpers.CapitalizeFirstLetterAndLowercaseRest(request.FirstName),
                request.Username.ToLower(),
                request.Email.ToLower(),
                invitationCode,
                bulkUserUploadRequest.ActiveCohortId
            );

            await using var db = new DissertationDbContext(this._dbOptions, this._httpContextAccessor);
            await db.StudentInvites.AddAsync(studentInvite);
            db.SaveChanges();

            //publish email
            await PublishStudentInviteMessage(request, invitationCode, sender, client);
        }
    }

    private async Task PublishSupervisionInviteMessage(UserUploadRequest request,
        string invitationCode, ServiceBusSender sender,  ServiceBusClient client)
    {
        ApplicationUrlSettings? applicationUrlSettings = this._configuration.GetSection("ApplicationUrlSettings").Get<ApplicationUrlSettings>();
        if (applicationUrlSettings == null)
        {
            throw new InvalidOperationException("ApplicationUrlSettings must be configured in app settings.");
        }

        //send an email to the user's email
        var callbackUrl = CallbackUrlGenerator.GenerateSupervisionInviteCallBackUrl(
            applicationUrlSettings.WebClientUrl, applicationUrlSettings.SupervisorConfirmInviteRoute,
            request.Username, invitationCode);

        var userDto = new UserDto
        {
            UserName = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email
        };
        var emailDto = new PublishEmailDto { User = userDto, CallbackUrl = callbackUrl, EmailType = EmailType.EmailTypeSupervisorInviteEmail };

        var jsonMessage = JsonConvert.SerializeObject(emailDto);
        var finalMessage = new ServiceBusMessage(Encoding
            .UTF8.GetBytes(jsonMessage))
        {
            CorrelationId = Guid.NewGuid().ToString(),
        };

        await sender.SendMessageAsync(finalMessage);
        await client.DisposeAsync();
    }

    private async Task PublishStudentInviteMessage(UserUploadRequest request,
        string invitationCode, ServiceBusSender sender,  ServiceBusClient client)
    {
        ApplicationUrlSettings? applicationUrlSettings = this._configuration.GetSection("ApplicationUrlSettings").Get<ApplicationUrlSettings>();
        if (applicationUrlSettings == null)
        {
            throw new InvalidOperationException("ApplicationUrlSettings must be configured in app settings.");
        }

        //send an email to the user's email
        var callbackUrl = CallbackUrlGenerator.GenerateStudentInviteCallBackUrl(
            applicationUrlSettings.WebClientUrl, applicationUrlSettings.SupervisorConfirmInviteRoute,
            request.Username, invitationCode);

        var userDto = new UserDto
        {
            UserName = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email
        };
        var emailDto = new PublishEmailDto { User = userDto, CallbackUrl = callbackUrl, EmailType = EmailType.EmailTypeStudentInviteEmail };

        var jsonMessage = JsonConvert.SerializeObject(emailDto);
        var finalMessage = new ServiceBusMessage(Encoding
            .UTF8.GetBytes(jsonMessage))
        {
            CorrelationId = Guid.NewGuid().ToString(),
        };

        await sender.SendMessageAsync(finalMessage);
        await client.DisposeAsync();
    }
}