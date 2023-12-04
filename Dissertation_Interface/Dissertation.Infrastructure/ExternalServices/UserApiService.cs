using System.Text.Json;
using System.Text.Json.Serialization;
using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Helpers;
using Microsoft.Extensions.Options;
using Shared.DTO;
using Shared.Enums;
using Shared.Settings;
using UserManagement_API.Data.Models.Dto;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Dissertation.Infrastructure.ExternalServices;

public class UserApiService : IUserApiService
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

    public UserApiService(IRequestHelper requestHelper, IOptions<ServiceUrlSettings> serviceUrlSettings)
    {
        this._requestHelper = requestHelper;
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

    public async Task<ResponseDto<PaginatedUserListDto>> GetListOfSupervisors(SupervisorPaginationParameters model)
    {
        var urlParams = new Dictionary<string, object>
        {
            { "pageNumber", model.PageNumber }, { "pageSize", model.PageSize },
            { "searchByUserName", model.SearchByUserName },
            { "filterByDepartment", model.FilterByDepartment }
        };

        var url = $"{this._serviceUrlSettings.UserApi}{UserApiUrlRoutes.GetSupervisors}";
        var response = await this._requestHelper.GetAsync(url, null, urlParams, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<PaginatedUserListDto>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<PaginatedUserListDto>> GetListOfStudents(DissertationStudentPaginationParameters model)
    {
        var urlParams = new Dictionary<string, object>
        {
            { "pageNumber", model.PageNumber }, { "pageSize", model.PageSize },
            { "searchByUserName", model.SearchByUserName },  { "filterByCourse", model.FilterByCourse },
            { "cohortEndDate", model.CohortEndDate }, { "cohortStartDate", model.CohortStartDate }
        };

        var url = $"{this._serviceUrlSettings.UserApi}{UserApiUrlRoutes.GetStudents}";
        var response = await this._requestHelper.GetAsync(url, null, urlParams, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<PaginatedUserListDto>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<GetUserDto>> GetUserByUserId(string userId)
    {
        var url = $"{this._serviceUrlSettings.UserApi}{UserApiUrlRoutes.UserRoute}{userId}";
        var response = await this._requestHelper.GetAsync(url, null, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<GetUserDto>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<UserDto>> EditStudent(EditStudentRequestDto model)
    {
        var url = $"{this._serviceUrlSettings.UserApi}{UserApiUrlRoutes.EditStudentRoute}";
        var response = await this._requestHelper.PostAsync(url, model, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<UserDto>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<UserDto>> EditSupervisor(EditSupervisorRequestDto model)
    {
        var url = $"{this._serviceUrlSettings.UserApi}{UserApiUrlRoutes.EditSupervisorRoute}";
        var response = await this._requestHelper.PostAsync(url, model, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<UserDto>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<UserDto>> AssignAdminRoleToSupervisor (AssignAdminRoleRequestDto model)
    {
        var url = $"{this._serviceUrlSettings.UserApi}{UserApiUrlRoutes.AssignAdminRoleToSupervisor}";
        var response = await this._requestHelper.PostAsync(url, model, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<UserDto>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<UserDto>> AssignSupervisorRoleToAdmin (AssignSupervisorRoleRequestDto model)
    {
        var url = $"{this._serviceUrlSettings.UserApi}{UserApiUrlRoutes.AssignSupervisorRoleToAdmin}";
        var response = await this._requestHelper.PostAsync(url, model, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<UserDto>>(response, this._jsonSerializerOptions)!;
    }
}