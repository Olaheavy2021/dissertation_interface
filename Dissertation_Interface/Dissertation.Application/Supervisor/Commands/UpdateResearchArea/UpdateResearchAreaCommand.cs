using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Supervisor.Commands.UpdateResearchArea;

public sealed record UpdateResearchAreaCommand(
    string ResearchArea
    ) : IRequest<ResponseDto<SupervisorDto>>;