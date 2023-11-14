using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Department.Commands.CreateDepartment;

public sealed record CreateDepartmentCommand(
    string Name
    ): IRequest<ResponseDto<GetDepartment>>;
