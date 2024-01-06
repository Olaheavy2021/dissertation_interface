using Dissertation.Application.DTO.Request;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.SupervisionList.Queries;

public sealed record GetSupervisionListQuery(AdminSupervisionListParameters Parameters) : IRequest<ResponseDto<PaginatedSupervisionListDto>>;