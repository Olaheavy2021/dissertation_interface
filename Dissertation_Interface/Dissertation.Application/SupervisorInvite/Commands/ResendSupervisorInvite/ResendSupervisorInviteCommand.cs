using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.SupervisorInvite.Commands.ResendSupervisorInvite;

public sealed record ResendSupervisorInviteCommand(
    long InviteId
) : IRequest<ResponseDto<GetSupervisorInvite>>;