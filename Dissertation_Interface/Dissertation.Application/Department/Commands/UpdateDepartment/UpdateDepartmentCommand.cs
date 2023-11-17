using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Department.Commands.UpdateDepartment;

public sealed record UpdateDepartmentCommand(
    string Name,
    long DepartmentId
    ): IRequest<ResponseDto<GetDepartment>>;