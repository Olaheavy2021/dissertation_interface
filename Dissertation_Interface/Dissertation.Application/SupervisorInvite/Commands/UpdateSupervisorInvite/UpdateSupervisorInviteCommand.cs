using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.SupervisorInvite.Commands.UpdateSupervisorInvite;

public sealed record UpdateSupervisorInviteCommand(
    string LastName,
    string FirstName,
    string StaffId,
    string Email,
    long Id
) : IRequest<ResponseDto<GetSupervisorInvite>>;