using Dissertation.Infrastructure.DTO;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.SupervisorInvite.Commands.UploadSupervisorInvite;

public sealed record UploadSupervisorInviteCommand(
    List<UserUploadRequest> Requests
    ) : IRequest<ResponseDto<string>>;