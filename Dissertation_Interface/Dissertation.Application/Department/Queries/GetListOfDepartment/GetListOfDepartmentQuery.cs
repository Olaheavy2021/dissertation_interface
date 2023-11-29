using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Pagination;
using MediatR;
using Shared.DTO;
using Shared.Helpers;

namespace Dissertation.Application.Department.Queries.GetListOfDepartment;

public sealed record GetListOfDepartmentQuery(DepartmentPaginationParameters Parameters) : IRequest<ResponseDto<PaginatedDepartmentListDto>>;