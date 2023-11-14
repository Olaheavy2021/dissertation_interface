using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.DissertationCohort.Queries.GetActiveDissertationCohort;

public sealed record GetActiveDissertationCohortQuery : IRequest<ResponseDto<GetDissertationCohort>>;
