using MediatR;
using Shared.DTO;

namespace Dissertation.Application.SupervisorSuggestion.Commands.InitiateMatching;

public sealed record InitiateMatchingCommand : IRequest<ResponseDto<string>>;