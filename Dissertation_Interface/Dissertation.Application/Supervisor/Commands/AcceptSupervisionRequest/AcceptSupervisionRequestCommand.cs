using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Supervisor.Commands.AcceptSupervisionRequest;

public sealed record AcceptSupervisionRequestCommand(
    long RequestId,
    string Comment
    ): IRequest<ResponseDto<string>>;