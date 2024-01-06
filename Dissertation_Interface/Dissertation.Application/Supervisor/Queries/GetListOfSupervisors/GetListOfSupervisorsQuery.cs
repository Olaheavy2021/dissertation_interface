using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Supervisor.Queries.GetListOfSupervisors;

public sealed record GetListOfSupervisorsQuery(SupervisorPaginationParameters Parameters) : IRequest<ResponseDto<PaginatedSupervisorListDto>>;