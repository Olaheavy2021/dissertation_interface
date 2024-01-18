using System.Text;
using Azure.Messaging.ServiceBus;
using Dissertation.Infrastructure.DTO;
using Dissertation.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shared.Constants;
using Shared.Logging;
using Shared.Settings;

namespace Dissertation.Infrastructure.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly BatchUploadService _batchUploadService;
    private readonly ServiceBusSettings _serviceBusSettings;
    private readonly ServiceBusProcessor _batchUploadProcessor;
    private readonly IAppLogger<AzureServiceBusConsumer> _logger;

    public AzureServiceBusConsumer
        (BatchUploadService batchUploadService, IOptions<ServiceBusSettings> serviceBusSettings,
            IAppLogger<AzureServiceBusConsumer> logger)
    {
        this._batchUploadService = batchUploadService;
        this._logger = logger;
        this._serviceBusSettings = serviceBusSettings.Value;

        var client = new ServiceBusClient(this._serviceBusSettings.ServiceBusConnectionString);
        this._batchUploadProcessor = client.CreateProcessor(this._serviceBusSettings.BatchUploadQueue);
    }

    #region Processor Methods
    public async Task Start()
    {
        this._batchUploadProcessor.ProcessMessageAsync += OnMessageReceived;
        this._batchUploadProcessor.ProcessErrorAsync += ErrorHandler;
        await this._batchUploadProcessor.StartProcessingAsync();
    }

    public async Task Stop()
    {
        await this._batchUploadProcessor.StopProcessingAsync();
        await this._batchUploadProcessor.DisposeAsync();
    }
    #endregion

    private async Task OnMessageReceived(ProcessMessageEventArgs args)
    {
        try
        {
            // deserialize the incoming request
            ServiceBusReceivedMessage? message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            BulkUserUploadRequest? request = JsonConvert.DeserializeObject<BulkUserUploadRequest>(body);

            switch (request?.BatchUploadType)
            {
                case BatchUploadType.StudentInvite:
                    await this._batchUploadService.ProcessStudentInvites(request);
                    break;
                case BatchUploadType.SupervisorInvite:
                    await this._batchUploadService.ProcessSupervisorInvites(request);
                    break;
                default:
                    this._logger.LogInformation("An invalid Upload Type was passed");
                    break;
            }

        }
        catch (Exception ex)
        {
            this._logger.LogError("An exception occurred whilst processing the messages on the batch upload queue - {0}", ex);
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        this._logger.LogError("An error occurred while processing messages on the batch upload queue", args.Exception.ToString());
        return Task.CompletedTask;
    }
}