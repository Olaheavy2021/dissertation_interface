using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.StudentInvite.Commands.UpdateStudentInvite;

public sealed record UpdateStudentInviteCommand(
    string LastName,
    string FirstName,
    string StudentId,
    string Email,
    long Id
) : IRequest<ResponseDto<GetStudentInvite>>;