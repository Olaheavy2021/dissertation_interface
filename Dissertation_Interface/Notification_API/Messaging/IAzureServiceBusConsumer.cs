namespace Notification_API.Messaging;

public interface IAzureServiceBusConsumer
{
    Task Start();
    Task Stop();
}