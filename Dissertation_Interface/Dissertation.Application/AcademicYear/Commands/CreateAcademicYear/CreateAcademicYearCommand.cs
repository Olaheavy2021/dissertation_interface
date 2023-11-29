using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.AcademicYear.Commands.CreateAcademicYear;

public sealed record CreateAcademicYearCommand(
    DateTime StartDate,
    DateTime EndDate
) : IRequest<ResponseDto<GetAcademicYear>>;