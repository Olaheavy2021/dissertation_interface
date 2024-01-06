using MediatR;
using Shared.DTO;

namespace Dissertation.Application.SupervisionCohort.Commands.CreateSupervisionCohort;

public sealed record CreateSupervisionCohortCommand(
    List<CreateSupervisionCohortRequest> SupervisionCohortRequests
    ) : IRequest<ResponseDto<string>>;