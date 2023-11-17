using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Department.Commands.DisableDepartment;

public sealed record DisableDepartmentCommand(
    long DepartmentId
): IRequest<ResponseDto<GetDepartment>>;