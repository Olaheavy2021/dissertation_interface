using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Student.Commands.InitiateSupervisionRequest;

public sealed record InitiateSupervisionRequestCommand(string SupervisorId) : IRequest<ResponseDto<string>>;