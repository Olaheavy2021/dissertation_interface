using MediatR;
using Shared.DTO;

namespace Dissertation.Application.SupervisorSuggestion.Commands.ProcessMatching;

public sealed record ProcessMatchingCommand(
    string TaskId
    ) : IRequest<InitiateMatchingResponse>;