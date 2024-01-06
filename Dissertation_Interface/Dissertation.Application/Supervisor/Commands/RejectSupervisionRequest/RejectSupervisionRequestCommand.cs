using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Supervisor.Commands.RejectSupervisionRequest;

public sealed record RejectSupervisionRequestCommand(
    long RequestId,
    string Comment
    ) : IRequest<ResponseDto<string>>;