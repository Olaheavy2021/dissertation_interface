using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Department.Commands.EnableDepartment;

public sealed record EnableDepartmentCommand(
    long DepartmentId
    ) : IRequest<ResponseDto<GetDepartment>>;