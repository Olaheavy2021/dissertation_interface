using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.SupervisorInvite.Commands.ConfirmSupervisorInvite;

public sealed record ConfirmSupervisorInviteCommand(
    string StaffId,
    string InvitationCode
) : IRequest<ResponseDto<GetSupervisorInvite>>;