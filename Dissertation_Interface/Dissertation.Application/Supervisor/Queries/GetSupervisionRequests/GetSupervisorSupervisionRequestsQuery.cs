using Dissertation.Application.DTO.Request;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Supervisor.Queries.GetSupervisionRequests;

public sealed record GetSupervisorSupervisionRequestsQuery(SupervisorSupervisionRequestParameters Parameters): IRequest<ResponseDto<PaginatedSupervisionRequestListDto>>;