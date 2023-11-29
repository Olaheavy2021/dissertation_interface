using System.Text.Json;
using System.Text.Json.Serialization;
using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.DTO;
using Shared.Enums;
using Shared.Settings;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Dissertation.Infrastructure.ExternalServices;

public class UserApiService : IUserApiService
{
    private readonly IRequestHelper _requestHelper;
    private readonly ILogger<UserApiService> _logger;
    private readonly ServiceUrlSettings _serviceUrlSettings;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public UserApiService(IRequestHelper requestHelper, ILogger<UserApiService> logger, IOptions<ServiceUrlSettings> serviceUrlSettings)
    {
        this._requestHelper = requestHelper;
        this._logger = logger;
        this._serviceUrlSettings = serviceUrlSettings.Value;
    }

    public async Task<ResponseDto<GetUserDto>> GetUserByEmail(string email)
    {
        var url = $"{this._serviceUrlSettings.UserApi}{UserApiUrlRoutes.GetUserByEmailRoute}{email}";
        var response = await this._requestHelper.GetAsync(url, null, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<GetUserDto>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<GetUserDto>> GetUserByUserName(string username)
    {
        var url = $"{this._serviceUrlSettings.UserApi}{UserApiUrlRoutes.GetUserByUserNameRoute}{username}";
        var response = await this._requestHelper.GetAsync(url, null, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<GetUserDto>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<string>> RegisterSupervisor(StudentOrSupervisorRegistrationDto model)
    {
        var url = $"{this._serviceUrlSettings.UserApi}{UserApiUrlRoutes.RegisterSupervisorRoute}";
        var response = await this._requestHelper.PostAsync(url, model, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<string>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<string>> RegisterStudent(StudentOrSupervisorRegistrationDto model)
    {
        var url = $"{this._serviceUrlSettings.UserApi}{UserApiUrlRoutes.RegisterStudentRoute}";
        var response = await this._requestHelper.PostAsync(url, model, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<string>>(response, this._jsonSerializerOptions)!;
    }
}