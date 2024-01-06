namespace Dissertation.Infrastructure.Messaging;

public interface IAzureServiceBusConsumer
{
    Task Start();
    Task Stop();
}