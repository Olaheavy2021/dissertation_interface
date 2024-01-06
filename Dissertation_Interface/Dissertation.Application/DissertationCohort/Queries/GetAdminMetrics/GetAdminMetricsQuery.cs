using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.DissertationCohort.Queries.GetAdminMetrics;

public sealed record GetAdminMetricsQuery : IRequest<ResponseDto<AdminMetricsResponse>>;