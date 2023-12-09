using Shared.DTO;
using UserManagement_API.Data.Models;

namespace UserManagement_API.Service.IService;

public interface ISupervisionCohortService
{
    Task<ResponseDto<string>> CreateSupervisionCohort(CreateSupervisionCohortListRequest request,
        CancellationToken cancellationToken);

    Task<ResponseDto<GetSupervisionCohort>> GetSupervisionCohort(long id);

    Task<ResponseDto<PaginatedSupervisionCohortListDto>> GetSupervisionCohorts(
        SupervisionCohortListParameters parameters);

    ResponseDto<PaginatedUserListDto> GetActiveSupervisorsForCohort(SupervisionCohortListParameters paginationParameters);

    ResponseDto<PaginatedUserListDto> GetInActiveSupervisorsForCohort(SupervisionCohortListParameters paginationParameters);

    Task<ResponseDto<SupervisionCohort>> GetSupervisionCohort(SupervisionCohortParameters parameters);
}