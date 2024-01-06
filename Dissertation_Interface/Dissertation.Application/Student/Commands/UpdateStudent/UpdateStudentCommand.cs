using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Student.Commands.UpdateStudent;

public sealed record UpdateStudentCommand(
    string LastName,
    string FirstName,
    string StudentId,
    long CourseId,
    long Id
    ) : IRequest<ResponseDto<UserDto>>;