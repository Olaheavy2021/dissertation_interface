using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Course.Commands.UpdateCourse;

public sealed record UpdateCourseCommand(
    long Id,
    string Name,
    long DepartmentId
) : IRequest<ResponseDto<GetCourse>>;