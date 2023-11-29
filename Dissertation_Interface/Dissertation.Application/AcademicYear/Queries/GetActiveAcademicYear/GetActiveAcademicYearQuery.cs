using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.AcademicYear.Queries.GetActiveAcademicYear;

public sealed record GetActiveAcademicYearQuery() : IRequest<ResponseDto<GetAcademicYear>>;