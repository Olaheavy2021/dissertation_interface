using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Course.Commands.CreateCourse;

public sealed record CreateCourseCommand(
    string Name,
    long DepartmentId
    ): IRequest<ResponseDto<GetCourse>>;