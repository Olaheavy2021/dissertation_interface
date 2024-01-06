using Shared.DTO;

namespace UserManagement_API.Service.IService;

public interface IDissertationApiService
{
    Task<ResponseDto<GetDissertationCohort>> GetActiveDissertationCohort();

    Task<ResponseDto<IReadOnlyList<GetCourse>>> GetAllCourses();

    Task<ResponseDto<IReadOnlyList<GetDepartment>>> GetAllDepartments();
}