using Dissertation.Application.DTO.Request;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Supervisor.Queries.GetSupervisionLists;

public sealed record GetSupervisorSupervisionListQuery(SupervisorSupervisionListParameters Parameters): IRequest<ResponseDto<PaginatedSupervisionListDto>>;