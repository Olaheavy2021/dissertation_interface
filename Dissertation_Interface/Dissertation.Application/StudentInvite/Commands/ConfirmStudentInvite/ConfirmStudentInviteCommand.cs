using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.StudentInvite.Commands.ConfirmStudentInvite;

public sealed record ConfirmStudentInviteCommand(
    string StudentId,
    string InvitationCode
) : IRequest<ResponseDto<GetStudentInvite>>;