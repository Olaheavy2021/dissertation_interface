using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Supervisor.Commands.AssignAdminRole;

public sealed record AssignAdminRoleCommand(
    string Email,
    string Role
) : IRequest<ResponseDto<UserDto>>;