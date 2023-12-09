using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Department.Queries.GetAllDepartments;

public sealed record GetAllDepartmentsQuery : IRequest<ResponseDto<IReadOnlyList<GetDepartment>>>;