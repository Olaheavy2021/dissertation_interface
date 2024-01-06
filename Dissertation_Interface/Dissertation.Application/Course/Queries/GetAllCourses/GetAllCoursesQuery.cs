using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Course.Queries.GetAllCourses;

public sealed record GetAllCoursesQuery : IRequest<ResponseDto<IReadOnlyList<GetCourse>>>;