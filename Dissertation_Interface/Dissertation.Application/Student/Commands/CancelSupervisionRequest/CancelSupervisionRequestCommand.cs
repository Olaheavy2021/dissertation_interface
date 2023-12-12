using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Student.Commands.CancelSupervisionRequest;

public sealed record CancelSupervisionRequestCommand(
    long RequestId,
    string Comment
    ): IRequest<ResponseDto<string>>;