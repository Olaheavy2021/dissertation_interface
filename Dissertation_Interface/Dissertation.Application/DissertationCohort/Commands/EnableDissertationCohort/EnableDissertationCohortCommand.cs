using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.DissertationCohort.Commands.EnableDissertationCohort;

public sealed record EnableDissertationCohortCommand(
    long DissertationCohortId
    ) : IRequest<ResponseDto<GetDissertationCohort>>;