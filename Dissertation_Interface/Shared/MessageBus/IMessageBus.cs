namespace Shared.MessageBus;

public interface IMessageBus
{
    Task PublishMessage(object message, string topicQueueName, string connectionString);
}