using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.AcademicYear.Queries.GetById;

public sealed record GetAcademicYearByIdQuery(long AcademicYearId) : IRequest< ResponseDto<GetAcademicYear>>;