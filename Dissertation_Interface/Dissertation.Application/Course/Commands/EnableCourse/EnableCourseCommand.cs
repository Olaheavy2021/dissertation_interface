using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Course.Commands.EnableCourse;

public sealed record EnableCourseCommand(
    long CourseId
    ) : IRequest<ResponseDto<GetCourse>>;