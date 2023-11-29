using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Supervisor.Commands.RegisterSupervisor;

public sealed record RegisterSupervisorCommand(
    string FirstName,
    string LastName,
    string StaffId,
    long DepartmentId,
    string InvitationCode,
    string Password
): IRequest<ResponseDto<string>>;