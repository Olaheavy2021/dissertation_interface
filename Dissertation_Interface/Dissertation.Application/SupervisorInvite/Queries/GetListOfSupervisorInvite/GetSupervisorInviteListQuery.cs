using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Pagination;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.SupervisorInvite.Queries.GetListOfSupervisorInvite;

public sealed record GetSupervisorInviteListQuery(SupervisorInvitePaginationParameters Parameters) : IRequest<ResponseDto<PaginatedSupervisorInvite>>;