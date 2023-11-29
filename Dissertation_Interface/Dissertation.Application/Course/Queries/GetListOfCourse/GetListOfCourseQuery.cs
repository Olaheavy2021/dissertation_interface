using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Pagination;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Course.Queries.GetListOfCourse;

public sealed record GetListOfCourseQuery(CoursePaginationParameters Parameters) : IRequest<ResponseDto<PaginatedCourseListDto>>;