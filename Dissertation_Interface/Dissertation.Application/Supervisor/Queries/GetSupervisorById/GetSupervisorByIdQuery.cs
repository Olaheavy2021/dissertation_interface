using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Supervisor.Queries.GetSupervisorById;

public sealed record GetSupervisorByIdQuery(string Id) : IRequest<ResponseDto<GetSupervisor>>;