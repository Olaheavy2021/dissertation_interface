using System.Text;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace Shared.MessageBus;

public class MessageBus : IMessageBus
{
    public async Task PublishMessage(object message, string topicQueueName,string connectionString)
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
}