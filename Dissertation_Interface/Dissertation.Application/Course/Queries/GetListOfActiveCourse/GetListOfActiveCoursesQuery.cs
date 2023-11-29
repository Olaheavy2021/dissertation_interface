using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Course.Queries.GetListOfActiveCourse;

public sealed record GetListOfActiveCoursesQuery() : IRequest<ResponseDto<IEnumerable<GetCourse>>>;