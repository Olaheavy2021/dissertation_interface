using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Course.Queries.GetById;

public sealed record GetCourseByIdQuery(long CourseId) : IRequest<ResponseDto<GetCourse>>;