using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Student.Queries.GetAvailableSupervisors;

public sealed record GetAvailableSupervisorsQuery(
    SupervisionCohortListParameters Parameters
) : IRequest<ResponseDto<PaginatedSupervisionCohortListDto>>;