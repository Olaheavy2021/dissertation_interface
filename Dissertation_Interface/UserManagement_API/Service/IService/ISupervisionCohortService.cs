using Shared.DTO;
using UserManagement_API.Data.Models;

namespace UserManagement_API.Service.IService;

public interface ISupervisionCohortService
{
    Task<ResponseDto<string>> CreateSupervisionCohort(CreateSupervisionCohortListRequest request,
        CancellationToken cancellationToken);

    Task<ResponseDto<string>> UpdateSupervisionSlot(UpdateSupervisionCohortRequest request,
        CancellationToken cancellationToken);

    Task<ResponseDto<GetSupervisionCohort>> GetSupervisionCohort(long id);

    Task<ResponseDto<PaginatedSupervisionCohortListDto>> GetSupervisionCohorts(
        SupervisionCohortListParameters parameters);

    ResponseDto<PaginatedUserListDto> GetActiveSupervisorsForCohort(SupervisionCohortListParameters paginationParameters);

    ResponseDto<PaginatedUserListDto> GetInActiveSupervisorsForCohort(SupervisionCohortListParameters paginationParameters);
    Task<ResponseDto<SupervisionCohort>> GetSupervisionCohort(SupervisionCohortParameters parameters);
    Task<ResponseDto<string>> DeleteSupervisionCohort(long supervisionCohortId, CancellationToken cancellationToken);
    Task<ResponseDto<SupervisionCohortMetricsDto>> GetSupervisionCohortMetrics(long dissertationCohortId);
    Task<ResponseDto<IReadOnlyList<GetSupervisionCohort>>> GetAllSupervisionCohort(long cohortId);
}