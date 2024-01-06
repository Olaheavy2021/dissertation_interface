using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.SupervisionCohort.Queries.GetById;

public sealed record GetAssignedSupervisorByIdQuery(long Id) : IRequest<ResponseDto<GetSupervisionCohortDetails>>;