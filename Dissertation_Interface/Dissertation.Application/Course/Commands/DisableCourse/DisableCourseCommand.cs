using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Course.Commands.DisableCourse;

public sealed record DisableCourseCommand(
    long CourseId
    ) : IRequest<ResponseDto<GetCourse>>;
