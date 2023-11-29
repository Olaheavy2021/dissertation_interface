using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Pagination;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.StudentInvite.Queries.GetListOfStudentInviteQueryHandler;

public sealed record GetStudentInviteListQuery(StudentInvitePaginationParameters Parameters) : IRequest<ResponseDto<PaginatedStudentInvite>>;