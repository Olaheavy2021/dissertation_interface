using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Helpers;
using Shared.Logging;

namespace Dissertation.Application.AcademicYear.Queries.GetListOfAcademicYear;

public class GetListOfAcademicYearQueryHandler : IRequestHandler<GetListOfAcademicYearQuery, ResponseDto<PaginatedAcademicYearListDto>>
{
    private readonly IAppLogger<GetListOfAcademicYearQueryHandler> _logger;
    private readonly IUnitOfWork _db;

    public GetListOfAcademicYearQueryHandler(IAppLogger<GetListOfAcademicYearQueryHandler> logger, IUnitOfWork db)
    {
        this._db = db;
        this._logger = logger;
    }
    public Task<ResponseDto<PaginatedAcademicYearListDto>> Handle(GetListOfAcademicYearQuery request,
        CancellationToken cancellationToken)
    {
        var response = new ResponseDto<PaginatedAcademicYearListDto>();
        this._logger.LogInformation("Attempting to retrieve list of AcademicYear");
        PagedList<Domain.Entities.AcademicYear> academicYears =  this._db.AcademicYearRepository.GetListOfAcademicYears(request.Parameters);

        var mappedAcademicYears = new PagedList<GetAcademicYear>(
            academicYears.Select(MapToAcademicYearDto).ToList(),
            academicYears.TotalCount,
            academicYears.CurrentPage,
            academicYears.PageSize
        );

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = new PaginatedAcademicYearListDto
        {
            Data = mappedAcademicYears,
            TotalCount = mappedAcademicYears.TotalCount,
            PageSize = mappedAcademicYears.PageSize,
            CurrentPage = mappedAcademicYears.CurrentPage,
            TotalPages = mappedAcademicYears.TotalPages,
            HasNext = mappedAcademicYears.HasNext,
            HasPrevious = mappedAcademicYears.HasPrevious
        };

        return Task.FromResult(response);
    }

    private static GetAcademicYear MapToAcademicYearDto(Domain.Entities.AcademicYear academicYear) =>
        new()
        {
            Id = academicYear.Id,
            Status = academicYear.Status,
            StartDate = academicYear.StartDate,
            CreatedAt = academicYear.CreatedAt,
            EndDate = academicYear.EndDate,
            CreatedBy = academicYear.CreatedBy,
            UpdatedAt = academicYear.UpdatedAt,
            UpdatedBy = academicYear.UpdatedBy
        };
}