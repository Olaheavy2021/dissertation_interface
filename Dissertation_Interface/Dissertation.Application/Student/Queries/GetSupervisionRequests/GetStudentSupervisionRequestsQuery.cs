using Dissertation.Application.DTO.Request;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Student.Queries.GetSupervisionRequests;

public sealed record GetStudentSupervisionRequestsQuery(StudentSupervisionRequestParameters Parameters) : IRequest<ResponseDto<PaginatedSupervisionRequestListDto>>;