using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Pagination;
using MediatR;
using Shared.DTO;
using Shared.Helpers;

namespace Dissertation.Application.AcademicYear.Queries.GetListOfAcademicYear;

public sealed record GetListOfAcademicYearQuery(AcademicYearPaginationParameters Parameters) : IRequest<ResponseDto<PaginatedAcademicYearListDto>>;