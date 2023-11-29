using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.DissertationCohort.Queries.GetById;

public sealed record GetDissertationCohortByIdQuery(long DissertationCohortId) : IRequest<ResponseDto<GetDissertationCohort>>;