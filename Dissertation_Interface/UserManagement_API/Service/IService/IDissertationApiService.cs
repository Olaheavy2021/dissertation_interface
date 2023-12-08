using Shared.DTO;

namespace UserManagement_API.Service.IService;

public interface IDissertationApiService
{
    Task<ResponseDto<GetDissertationCohort>> GetActiveDissertationCohort();
}