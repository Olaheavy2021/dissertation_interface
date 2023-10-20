using System.Text;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Shared.Constants;
using Shared.DTO;

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

    public async Task PublishAuditLog(string eventType, string connectionString, string loggedInAdminEmail,
        string outcome)
    {
        var auditLoggerDto = new AuditLogDto
        {
            EventTimeStamp = DateTime.UtcNow,
            Email = loggedInAdminEmail,
            Outcome = outcome,
            EventType = eventType
        };

        switch (eventType)
        {
            case EventType.ResendEmailConfirmation:
                auditLoggerDto.EventDescription = EventDescription.ResendEmailConfirmation;
                auditLoggerDto.TargetEntity = AuditLogTargetEntity.Users;
                break;
            case EventType.EditUser:
                auditLoggerDto.EventDescription = EventDescription.EditUser;
                auditLoggerDto.TargetEntity = AuditLogTargetEntity.Users;
                break;
        }

        await PublishMessage(auditLoggerDto, ServiceBusQueues.AuditLoggerQueue, connectionString);
    }
}