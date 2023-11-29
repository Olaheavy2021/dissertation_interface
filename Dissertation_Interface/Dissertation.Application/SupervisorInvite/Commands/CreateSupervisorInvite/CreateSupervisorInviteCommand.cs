using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.SupervisorInvite.Commands.CreateSupervisorInvite;

public sealed record CreateSupervisorInviteCommand(
    string LastName,
    string FirstName,
    string StaffId,
    string Email
    ) : IRequest<ResponseDto<GetSupervisorInvite>>;