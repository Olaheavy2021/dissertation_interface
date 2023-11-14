using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Pagination;
using MediatR;
using Shared.DTO;
using Shared.Helpers;

namespace Dissertation.Application.DissertationCohort.Queries.GetListOfDissertationCohort;

public sealed record GetListOfDissertationCohortQuery(DissertationCohortPaginationParameters Parameters) : IRequest<ResponseDto<PagedList<GetDissertationCohort>>>;