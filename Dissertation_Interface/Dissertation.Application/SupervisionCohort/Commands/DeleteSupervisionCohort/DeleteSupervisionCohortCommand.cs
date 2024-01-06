using MediatR;
using Shared.DTO;

namespace Dissertation.Application.SupervisionCohort.Commands.DeleteSupervisionCohort;

public sealed record DeleteSupervisionCohortCommand(long SupervisionCohortId) : IRequest<ResponseDto<string>>;