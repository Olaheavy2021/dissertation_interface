using System.Text.Json;
using System.Text.Json.Serialization;
using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Helpers;
using Microsoft.Extensions.Options;
using Shared.DTO;
using Shared.Enums;
using Shared.Settings;

namespace Dissertation.Infrastructure.ExternalServices;

public class DissertationMatchingService : IDissertationMatchingService
{
    private readonly IRequestHelper _requestHelper;
    private readonly ServiceUrlSettings _serviceUrlSettings;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public DissertationMatchingService(IRequestHelper requestHelper, IOptions<ServiceUrlSettings> serviceUrlSettings)
    {
        this._requestHelper = requestHelper;
        this._serviceUrlSettings = serviceUrlSettings.Value;
    }

    public async Task<InitiateMatchingResponse> ProcessData(InitiateMatchingRequest request)
    {
        var url = $"{this._serviceUrlSettings.DissertationMatchingApi}{DissertationMatchingRoutes.ProcessData}";
        var response = await this._requestHelper.PostAsync(url, request, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<InitiateMatchingResponse>(response, this._jsonSerializerOptions)!;
    }

    public async Task<MatchingStatusRootObject> CheckStatus(string taskId)
    {
        var url = $"{this._serviceUrlSettings.DissertationMatchingApi}{DissertationMatchingRoutes.CheckStatusOfTask}{taskId}";
        var response = await this._requestHelper.GetAsync(url, null, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<MatchingStatusRootObject>(response, this._jsonSerializerOptions)!;
    }
}