using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.StudentInvite.Commands.ResendStudentInvite;

public sealed record ResendStudentInviteCommand(
    long InviteId
    ) : IRequest<ResponseDto<GetStudentInvite>>;