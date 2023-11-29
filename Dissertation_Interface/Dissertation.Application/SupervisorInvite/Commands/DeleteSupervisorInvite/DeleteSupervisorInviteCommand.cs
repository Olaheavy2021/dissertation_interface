using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.SupervisorInvite.Commands.DeleteSupervisorInvite;

public sealed record DeleteSupervisorInviteCommand(
    long Id
) : IRequest<ResponseDto<GetSupervisorInvite>>;