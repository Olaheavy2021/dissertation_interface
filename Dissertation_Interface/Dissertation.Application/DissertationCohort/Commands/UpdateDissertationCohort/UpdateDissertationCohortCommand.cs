using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.DissertationCohort.Commands.UpdateDissertationCohort;

public sealed record UpdateDissertationCohortCommand(
    DateTime StartDate,
    DateTime EndDate,
    DateTime SupervisionChoiceDeadline,
    long AcademicYearId,
    long Id
) : IRequest<ResponseDto<GetDissertationCohort>>;