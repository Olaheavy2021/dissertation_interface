using Dissertation.Infrastructure.DTO;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.StudentInvite.Commands.UploadStudentInvite;

public sealed record UploadStudentInviteCommand(
    List<UserUploadRequest> Requests
    ): IRequest<ResponseDto<string>>;