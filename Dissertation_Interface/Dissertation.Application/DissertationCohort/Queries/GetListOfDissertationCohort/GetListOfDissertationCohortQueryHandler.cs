using Dissertation.Application.AcademicYear.Queries.GetListOfAcademicYear;
using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Helpers;
using Shared.Logging;

namespace Dissertation.Application.DissertationCohort.Queries.GetListOfDissertationCohort;

public class GetListOfDissertationCohortQueryHandler : IRequestHandler<GetListOfDissertationCohortQuery, ResponseDto<PagedList<GetDissertationCohort>>>
{
    private readonly IAppLogger<GetListOfDissertationCohortQueryHandler> _logger;
    private readonly IUnitOfWork _db;

    public GetListOfDissertationCohortQueryHandler(IAppLogger<GetListOfDissertationCohortQueryHandler> logger, IUnitOfWork db)
    {
        this._db = db;
        this._logger = logger;
    }

    public Task<ResponseDto<PagedList<GetDissertationCohort>>> Handle(GetListOfDissertationCohortQuery request,
        CancellationToken cancellationToken)
    {
        var response = new ResponseDto<PagedList<GetDissertationCohort>>();
        this._logger.LogInformation("Attempting to retrieve list of Dissertation Cohort");
        PagedList<Domain.Entities.DissertationCohort> dissertationCohort =  this._db.DissertationCohortRepository.GetListOfDissertationCohort(request.Parameters);

        var mappedDissertationCohort = new PagedList<GetDissertationCohort>(
            dissertationCohort.Select(MapToDissertationCohortDto).ToList(),
            dissertationCohort.TotalCount,
            dissertationCohort.CurrentPage,
            dissertationCohort.PageSize
        );

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedDissertationCohort;

        return Task.FromResult(response);
    }

    private static GetDissertationCohort MapToDissertationCohortDto(Domain.Entities.DissertationCohort dissertationCohort) =>
        new()
        {
            Id = dissertationCohort.Id,
            Status = dissertationCohort.Status,
            StartDate = dissertationCohort.StartDate,
            CreatedAt = dissertationCohort.CreatedAt,
            EndDate = dissertationCohort.EndDate,
            CreatedBy = dissertationCohort.CreatedBy,
            UpdatedAt = dissertationCohort.UpdatedAt,
            UpdatedBy = dissertationCohort.UpdatedBy,
            SupervisionChoiceDeadline = dissertationCohort.SupervisionChoiceDeadline
        };
}