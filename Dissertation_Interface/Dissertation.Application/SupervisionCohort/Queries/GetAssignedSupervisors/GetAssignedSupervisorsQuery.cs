using MediatR;
using Shared.DTO;

namespace Dissertation.Application.SupervisionCohort.Queries.GetAssignedSupervisors;

public sealed record GetAssignedSupervisorsQuery(
    SupervisionCohortListParameters Parameters
    ) : IRequest<ResponseDto<PaginatedSupervisionCohortListDto>>;