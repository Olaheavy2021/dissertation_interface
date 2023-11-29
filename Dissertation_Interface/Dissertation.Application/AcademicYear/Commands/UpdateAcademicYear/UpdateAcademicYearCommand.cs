using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.AcademicYear.Commands.UpdateAcademicYear;

public sealed record UpdateAcademicYearCommand(
    DateTime StartDate,
    DateTime EndDate,
    long Id
    ) : IRequest<ResponseDto<GetAcademicYear>>;