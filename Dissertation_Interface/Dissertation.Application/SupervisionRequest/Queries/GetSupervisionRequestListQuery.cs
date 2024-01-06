using Dissertation.Application.DTO.Request;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.SupervisionRequest.Queries;

public sealed record GetSupervisionRequestListQuery(AdminSupervisionRequestParameters Parameters) : IRequest<ResponseDto<PaginatedSupervisionRequestListDto>>;