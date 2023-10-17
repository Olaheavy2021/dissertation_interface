namespace Shared.MessageBus;

public interface IMessageBus
{
    Task PublishMessage(object message, string topicQueueName, string connectionString);

    Task PublishAuditLog(string eventType, string connectionString, string loggedInAdminEmail,
        string outcome);
}