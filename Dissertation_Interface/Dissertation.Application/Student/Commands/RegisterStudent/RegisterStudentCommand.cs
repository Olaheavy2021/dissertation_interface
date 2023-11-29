using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Student.Commands.RegisterStudent;

public sealed record RegisterStudentCommand(
    string FirstName,
    string LastName,
    string StudentId,
    long CourseId,
    string InvitationCode,
    string Password
) : IRequest<ResponseDto<string>>;