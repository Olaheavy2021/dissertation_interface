using Dissertation.Application.DTO.Request;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Student.Queries.GetSupervisionLists;

public sealed record GetStudentSupervisionListsQuery(StudentSupervisionListsParameters Parameters): IRequest<ResponseDto<PaginatedSupervisionListDto>>;