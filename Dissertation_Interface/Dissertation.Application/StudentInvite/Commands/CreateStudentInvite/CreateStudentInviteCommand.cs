using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.StudentInvite.Commands.CreateStudentInvite;

public sealed record CreateStudentInviteCommand(
    string LastName,
    string FirstName,
    string StudentId,
    string Email
    ) : IRequest<ResponseDto<GetStudentInvite>>;