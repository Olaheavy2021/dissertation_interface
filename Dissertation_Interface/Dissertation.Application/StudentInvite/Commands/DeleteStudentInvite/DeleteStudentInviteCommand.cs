using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.StudentInvite.Commands.DeleteStudentInvite;

public sealed record DeleteStudentInviteCommand(
    long Id
): IRequest<ResponseDto<GetStudentInvite>>;