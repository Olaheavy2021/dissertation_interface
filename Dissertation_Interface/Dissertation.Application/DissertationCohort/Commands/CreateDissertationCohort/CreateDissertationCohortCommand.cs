using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.DissertationCohort.Commands.CreateDissertationCohort;

public sealed record CreateDissertationCohortCommand(
    DateTime StartDate,
    DateTime EndDate,
    DateTime SupervisionChoiceDeadline,
    long AcademicYearId
    ) : IRequest<ResponseDto<GetDissertationCohort>>;