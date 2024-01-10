using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Shared.DTO;
using Shared.Enums;
using Shared.Settings;
using UserManagement_API.Helpers;
using UserManagement_API.Service.IService;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace UserManagement_API.ExternalServices;

[ExcludeFromCodeCoverage]
public class DissertationApiService : IDissertationApiService
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

    public DissertationApiService(IRequestHelper requestHelper, IOptions<ServiceUrlSettings> serviceUrlSettings)
    {
        this._requestHelper = requestHelper;
        this._serviceUrlSettings = serviceUrlSettings.Value;
    }

    public async Task<ResponseDto<GetDissertationCohort>> GetActiveDissertationCohort()
    {
        var url = $"{this._serviceUrlSettings.DissertationApi}{DissertationCohortRoutes.GetActiveDissertationCohortRoute}";
        var response = await this._requestHelper.GetAsync(url, null, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<GetDissertationCohort>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<IReadOnlyList<GetCourse>>> GetAllCourses()
    {
        var url = $"{this._serviceUrlSettings.DissertationApi}{CourseRoutes.GetAllCoursesRoute}";
        var response = await this._requestHelper.GetAsync(url, null, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<IReadOnlyList<GetCourse>>>(response, this._jsonSerializerOptions)!;
    }

    public async Task<ResponseDto<IReadOnlyList<GetDepartment>>> GetAllDepartments()
    {
        var url = $"{this._serviceUrlSettings.DissertationApi}{DepartmentRoutes.GetAllDepartmentsRoute}";
        var response = await this._requestHelper.GetAsync(url, null, mediaType: MediaType.Json);
        return JsonSerializer.Deserialize<ResponseDto<IReadOnlyList<GetDepartment>>>(response, this._jsonSerializerOptions)!;
    }
}