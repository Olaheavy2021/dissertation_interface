using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.SupervisorInvite.Queries.GetById;

public sealed record GetSupervisionInviteByIdQuery(long Id) : IRequest<ResponseDto<GetSupervisorInvite>>;