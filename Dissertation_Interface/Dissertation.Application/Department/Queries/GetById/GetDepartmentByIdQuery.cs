using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Department.Queries.GetById;

public sealed record GetDepartmentByIdQuery(long DepartmentId) : IRequest< ResponseDto<GetDepartment>>;
