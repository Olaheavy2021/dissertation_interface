using MediatR;
using Shared.DTO;

namespace Dissertation.Application.SupervisionCohort.Queries.GetUnassignedSupervisors;

public sealed record GetUnassignedSupervisorsQuery(SupervisionCohortListParameters Parameters
    ) : IRequest<ResponseDto<PaginatedUserListDto>>;