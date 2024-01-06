using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Supervisor.Commands.UpdateSupervisor;

public sealed record UpdateSupervisorCommand(
    string LastName,
    string FirstName,
    string StaffId,
    long DepartmentId,
    long Id
    ) : IRequest<ResponseDto<UserDto>>;