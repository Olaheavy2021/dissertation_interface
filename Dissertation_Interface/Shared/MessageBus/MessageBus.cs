using System.Text;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Shared.Constants;
using Shared.DTO;
using Shared.Helpers;

namespace Shared.MessageBus;

public class MessageBus : IMessageBus
{
    public async Task PublishMessage(object message, string topicQueueName, string connectionString)
    {
        await using var client = new ServiceBusClient(connectionString);

        ServiceBusSender sender = client.CreateSender(topicQueueName);

        var jsonMessage = JsonConvert.SerializeObject(message);
        var finalMessage = new ServiceBusMessage(Encoding
            .UTF8.GetBytes(jsonMessage))
        {
            CorrelationId = Guid.NewGuid().ToString(),
        };

        await sender.SendMessageAsync(finalMessage);
        await client.DisposeAsync();
    }

    public async Task PublishAuditLog(string eventType, string? connectionString, string? loggedInAdminEmail,
        string outcome, string entityIdentifier)
    {
        var auditLoggerDto = new AuditLogDto
        {
            EventTimeStamp = DateTime.UtcNow,
            AdminEmail = loggedInAdminEmail,
            Outcome = outcome,
            EventType = eventType
        };

        switch (eventType)
        {
            case EventType.ResendEmailConfirmation:
                auditLoggerDto.EventDescription = StringHelpers.ReplaceEmail(EventDescription.ResendEmailConfirmation, auditLoggerDto.EntityIdentifier);
                auditLoggerDto.TargetEntity = AuditLogTargetEntity.Users;
                break;
            case EventType.EditUser:
                auditLoggerDto.EventDescription = StringHelpers.ReplaceEmail(EventDescription.EditUser, auditLoggerDto.EntityIdentifier);
                auditLoggerDto.TargetEntity = AuditLogTargetEntity.Users;
                break;
            case EventType.LockOutUser:
                auditLoggerDto.EventDescription = StringHelpers.ReplaceEmail(EventDescription.LockOutUser, auditLoggerDto.EntityIdentifier);
                auditLoggerDto.TargetEntity = AuditLogTargetEntity.Users;
                break;
            case EventType.UnlockUser:
                auditLoggerDto.EventDescription = StringHelpers.ReplaceEmail(EventDescription.UnlockUser, auditLoggerDto.EntityIdentifier);
                auditLoggerDto.TargetEntity = AuditLogTargetEntity.Users;
                break;
            case EventType.RegisterAdminUser:
                auditLoggerDto.EventDescription = StringHelpers.ReplaceEmail(EventDescription.RegisterAdminUser, auditLoggerDto.EntityIdentifier);
                auditLoggerDto.TargetEntity = AuditLogTargetEntity.Users;
                break;
        }

        await PublishMessage(auditLoggerDto, ServiceBusQueues.AuditLoggerQueue, connectionString);
    }
}