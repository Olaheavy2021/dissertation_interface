using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Supervisor.Queries.GetActiveStudentsCohort;

public sealed record GetActiveStudentsCohortQuery(StudentPaginationParameters Parameters) : IRequest<ResponseDto<PaginatedStudentListDto>>;