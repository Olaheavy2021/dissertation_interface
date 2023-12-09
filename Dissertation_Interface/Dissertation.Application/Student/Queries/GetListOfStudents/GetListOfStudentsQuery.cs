using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Student.Queries.GetListOfStudents;

public sealed record GetListOfStudentsQuery(StudentPaginationParameters Parameters): IRequest<ResponseDto<PaginatedStudentListDto>>;