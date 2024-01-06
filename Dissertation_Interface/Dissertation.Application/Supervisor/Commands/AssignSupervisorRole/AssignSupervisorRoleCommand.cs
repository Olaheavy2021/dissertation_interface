using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Supervisor.Commands.AssignSupervisorRole;

public sealed record AssignSupervisorRoleCommand(
    string Email,
    long DepartmentId
) : IRequest<ResponseDto<UserDto>>;