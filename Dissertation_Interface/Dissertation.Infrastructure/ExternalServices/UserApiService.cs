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

    #region User
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

    public async Task<ResponseDto<GetUserDto>> GetUserByUserId(string userId)
    {
        var url = $"{this._serviceUrlSettings.UserApi}{UserApiUrlRoutes.UserRoute}{userId}";
        var response = await this._requestHelper.GetAsync(url, null, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<GetUserDto>>(response, this._jsonSerializerOptions)!;
    }
    #endregion

    #region Supervisor
    public async Task<ResponseDto<string>> RegisterSupervisor(StudentOrSupervisorRegistrationDto model)
    {
        var url = $"{this._serviceUrlSettings.UserApi}{UserApiUrlRoutes.RegisterSupervisorRoute}";
        var response = await this._requestHelper.PostAsync(url, model, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<string>>(response, this._jsonSerializerOptions)!;
    }
    public async Task<ResponseDto<PaginatedSupervisorListDto>> GetListOfSupervisors(SupervisorPaginationParameters model)
    {
        var urlParams = new Dictionary<string, object>
        {
            { "pageNumber", model.PageNumber }, { "pageSize", model.PageSize },
            { "searchByUserName", model.SearchByUserName },
            { "filterByDepartment", model.FilterByDepartment }
        };

        var url = $"{this._serviceUrlSettings.UserApi}{UserApiUrlRoutes.GetSupervisors}";
        var response = await this._requestHelper.GetAsync(url, null, urlParams, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<PaginatedSupervisorListDto>>(response, this._jsonSerializerOptions)!;
    }
    public async Task<ResponseDto<UserDto>> EditSupervisor(EditSupervisorRequestDto model)
    {
        var url = $"{this._serviceUrlSettings.UserApi}{UserApiUrlRoutes.EditSupervisorRoute}";
        var response = await this._requestHelper.PostAsync(url, model, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<UserDto>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<UserDto>> AssignAdminRoleToSupervisor(AssignAdminRoleRequestDto model)
    {
        var url = $"{this._serviceUrlSettings.UserApi}{UserApiUrlRoutes.AssignAdminRoleToSupervisor}";
        var response = await this._requestHelper.PostAsync(url, model, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<UserDto>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<UserDto>> AssignSupervisorRoleToAdmin(AssignSupervisorRoleRequestDto model)
    {
        var url = $"{this._serviceUrlSettings.UserApi}{UserApiUrlRoutes.AssignSupervisorRoleToAdmin}";
        var response = await this._requestHelper.PostAsync(url, model, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<UserDto>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<PaginatedSupervisionRequestListDto>> GetSupervisionRequestsForASupervisor(
        SupervisionRequestPaginationParameters model)
    {
        var urlParams = new Dictionary<string, object>
        {
            { "pageNumber", model.PageNumber }, { "pageSize", model.PageSize },
            { "filterByStatus", model.FilterByStatus },{ "dissertationCohortId", model.DissertationCohortId },
            {"searchByStudent", model.SearchByStudent},  {"supervisorId", model.SupervisorId}
        };

        var url = $"{this._serviceUrlSettings.UserApi}{SupervisionRequestRoutes.GetSupervisionRequestsForASupervisor}";
        var response = await this._requestHelper.GetAsync(url, null, urlParams, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<PaginatedSupervisionRequestListDto>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<PaginatedSupervisionListDto>> GetSupervisionListForASupervisor(
        SupervisionListPaginationParameters model)
    {
        var urlParams = new Dictionary<string, object>
        {
            { "pageNumber", model.PageNumber }, { "pageSize", model.PageSize },
            { "dissertationCohortId", model.DissertationCohortId },
            {"searchByStudent", model.SearchByStudent},  {"supervisorId", model.SupervisorId}
        };

        var url = $"{this._serviceUrlSettings.UserApi}{SupervisionListRoutes.GetSupervisionListsForASupervisor}";
        var response = await this._requestHelper.GetAsync(url, null, urlParams, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<PaginatedSupervisionListDto>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<string>> RejectSupervisionRequest(ActionSupervisionRequest request)
    {
        var url = $"{this._serviceUrlSettings.DissertationMatchingApi}{SupervisionRequestRoutes.RejectSupervisionRequest}";
        var response = await this._requestHelper.PostAsync(url, request, null, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<string>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<string>> AcceptSupervisionRequest(ActionSupervisionRequest request)
    {
        var url = $"{this._serviceUrlSettings.UserApi}{SupervisionRequestRoutes.AcceptSupervisionRequest}";
        var response = await this._requestHelper.PostAsync(url, request, null, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<string>>(response, this._jsonSerializerOptions)!;
    }

    #endregion

    #region Student
    public async Task<ResponseDto<string>> RegisterStudent(StudentOrSupervisorRegistrationDto model)
    {
        var url = $"{this._serviceUrlSettings.UserApi}{UserApiUrlRoutes.RegisterStudentRoute}";
        var response = await this._requestHelper.PostAsync(url, model, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<string>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<PaginatedStudentListDto>> GetListOfStudents(DissertationStudentPaginationParameters model)
    {
        var urlParams = new Dictionary<string, object>
        {
            { "pageNumber", model.PageNumber }, { "pageSize", model.PageSize },
            { "searchByUserName", model.SearchByUserName },  { "filterByCourse", model.FilterByCourse },
            { "cohortEndDate", model.CohortEndDate }, { "cohortStartDate", model.CohortStartDate }
        };

        var url = $"{this._serviceUrlSettings.UserApi}{UserApiUrlRoutes.GetStudents}";
        var response = await this._requestHelper.GetAsync(url, null, urlParams, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<PaginatedStudentListDto>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<UserDto>> EditStudent(EditStudentRequestDto model)
    {
        var url = $"{this._serviceUrlSettings.UserApi}{UserApiUrlRoutes.EditStudentRoute}";
        var response = await this._requestHelper.PostAsync(url, model, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<UserDto>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<string>> CreateSupervisionRequest(CreateSupervisionRequest model)
    {
        var url = $"{this._serviceUrlSettings.UserApi}{SupervisionRequestRoutes.SupervisionRequestRoute}";
        var response = await this._requestHelper.PostAsync(url, model, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<string>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<PaginatedSupervisionRequestListDto>> GetSupervisionRequestsForStudents(
        SupervisionRequestPaginationParameters model)
    {
        var urlParams = new Dictionary<string, object>
        {
            { "pageNumber", model.PageNumber }, { "pageSize", model.PageSize },
            { "filterByStatus", model.FilterByStatus },{ "dissertationCohortId", model.DissertationCohortId },
            {"searchBySupervisor", model.SearchBySupervisor},  {"studentId", model.StudentId}
        };

        var url = $"{this._serviceUrlSettings.UserApi}{SupervisionRequestRoutes.GetSupervisionRequestsForAStudent}";
        var response = await this._requestHelper.GetAsync(url, null, urlParams, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<PaginatedSupervisionRequestListDto>>(response, this._jsonSerializerOptions)!;
    }
    public async Task<ResponseDto<string>> CancelSupervisionRequest(ActionSupervisionRequest request)
    {
        var url = $"{this._serviceUrlSettings.UserApi}{SupervisionRequestRoutes.CancelSupervisionRequest}";
        var response = await this._requestHelper.PostAsync(url, request, null, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<string>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<PaginatedSupervisionListDto>> GetSupervisionListsForStudents(
        SupervisionListPaginationParameters model)
    {
        var urlParams = new Dictionary<string, object>
        {
            { "pageNumber", model.PageNumber }, { "pageSize", model.PageSize },
            { "dissertationCohortId", model.DissertationCohortId }, {"searchBySupervisor", model.SearchBySupervisor},
            {"studentId", model.StudentId}
        };

        var url = $"{this._serviceUrlSettings.UserApi}{SupervisionListRoutes.GetSupervisionListsForAStudent}";
        var response = await this._requestHelper.GetAsync(url, null, urlParams, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<PaginatedSupervisionListDto>>(response, this._jsonSerializerOptions)!;
    }
    #endregion

    #region SupervisionCohort

    public async Task<ResponseDto<string>> CreateSupervisionCohortListRequest(CreateSupervisionCohortListRequest model)
    {
        var url = $"{this._serviceUrlSettings.UserApi}{SupervisionCohortRoutes.SupervisionCohortRoute}";
        var response = await this._requestHelper.PostAsync(url, model, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<string>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<PaginatedSupervisionCohortListDto>> GetSupervisionCohorts(
        SupervisionCohortListParameters model)
    {
        var urlParams = new Dictionary<string, object>
        {
            { "pageNumber", model.PageNumber }, { "pageSize", model.PageSize },
            { "searchByUserName", model.SearchByUserName },
            { "dissertationCohortId", model.DissertationCohortId }
        };

        var url = $"{this._serviceUrlSettings.UserApi}{SupervisionCohortRoutes.SupervisionCohortRoute}";
        var response = await this._requestHelper.GetAsync(url, null, urlParams, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<PaginatedSupervisionCohortListDto>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<PaginatedUserListDto>> GetUnAssignedSupervisors(
        SupervisionCohortListParameters model)
    {
        var urlParams = new Dictionary<string, object>
        {
            { "pageNumber", model.PageNumber }, { "pageSize", model.PageSize },
            { "searchByUserName", model.SearchByUserName },
            { "dissertationCohortId", model.DissertationCohortId }
        };

        var url = $"{this._serviceUrlSettings.UserApi}{SupervisionCohortRoutes.UnassignedSupervisors}";
        var response = await this._requestHelper.GetAsync(url, null, urlParams, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<PaginatedUserListDto>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<GetSupervisionCohort>> GetSupervisionCohort(long id)
    {
        var url = $"{this._serviceUrlSettings.UserApi}{SupervisionCohortRoutes.SupervisionCohortRoute}/{id}";
        var response = await this._requestHelper.GetAsync(url, null, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<GetSupervisionCohort>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<string>> UpdateSupervisionSlots(UpdateSupervisionCohortRequest request)
    {
        var url = $"{this._serviceUrlSettings.UserApi}{SupervisionCohortRoutes.UpdateSupervisionSlots}";
        var response = await this._requestHelper.PostAsync(url, request, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<string>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<string>> DeleteSupervisionCohort(long supervisionCohortId)
    {
        var url = $"{this._serviceUrlSettings.UserApi}{SupervisionCohortRoutes.SupervisionCohortRoute}/{supervisionCohortId}";
        var response = await this._requestHelper.DeleteAsync(url, null, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<string>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<SupervisionCohortMetricsDto>> GetSupervisionCohortMetrics(long dissertationCohortId)
    {
        var url = $"{this._serviceUrlSettings.UserApi}{SupervisionCohortRoutes.GetSupervisionCohortMetrics}{dissertationCohortId}";
        var response = await this._requestHelper.GetAsync(url, null, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<SupervisionCohortMetricsDto>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<List<GetSupervisionCohort>>> GetAllSupervisionCohort(long cohortId)
    {
        var url = $"{this._serviceUrlSettings.UserApi}{SupervisionCohortRoutes.GetAllSupervisionCohortRoute}{cohortId}";
        var response = await this._requestHelper.GetAsync(url, null, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<List<GetSupervisionCohort>>>(response, this._jsonSerializerOptions)!;
    }

    #endregion

    #region SupervisionRequest
    public async Task<ResponseDto<PaginatedSupervisionRequestListDto>> GetSupervisionRequests(
        SupervisionRequestPaginationParameters model)
    {
        var urlParams = new Dictionary<string, object>
        {
            { "pageNumber", model.PageNumber }, { "pageSize", model.PageSize },
            { "filterByStatus", model.FilterByStatus },{ "dissertationCohortId", model.DissertationCohortId },
            {"searchBySupervisor", model.SearchBySupervisor}
        };

        var url = $"{this._serviceUrlSettings.UserApi}{SupervisionRequestRoutes.SupervisionRequestRoute}";
        var response = await this._requestHelper.GetAsync(url, null, urlParams, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<PaginatedSupervisionRequestListDto>>(response, this._jsonSerializerOptions)!;
    }
    #endregion

    #region SupervisionList
    public async Task<ResponseDto<PaginatedSupervisionListDto>> GetSupervisionLists(
        SupervisionListPaginationParameters model)
    {
        var urlParams = new Dictionary<string, object>
        {
            { "pageNumber", model.PageNumber }, { "pageSize", model.PageSize },
            { "dissertationCohortId", model.DissertationCohortId },
            {"searchBySupervisor", model.SearchBySupervisor}
        };

        var url = $"{this._serviceUrlSettings.UserApi}{SupervisionListRoutes.SupervisionListRoute}";
        var response = await this._requestHelper.GetAsync(url, null, urlParams, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<PaginatedSupervisionListDto>>(response, this._jsonSerializerOptions)!;
    }
    #endregion
}