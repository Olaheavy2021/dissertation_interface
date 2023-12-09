using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Enums;
using Shared.Helpers;
using Shared.Logging;

namespace Dissertation.Application.DissertationCohort.Queries.GetListOfDissertationCohort;

public class GetListOfDissertationCohortQueryHandler : IRequestHandler<GetListOfDissertationCohortQuery, ResponseDto<PaginatedDissertationCohortListDto>>
{
    private readonly IAppLogger<GetListOfDissertationCohortQueryHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public GetListOfDissertationCohortQueryHandler(IAppLogger<GetListOfDissertationCohortQueryHandler> logger, IUnitOfWork db, IMapper mapper)
    {
        this._db = db;
        this._logger = logger;
        this._mapper = mapper;
    }

    public Task<ResponseDto<PaginatedDissertationCohortListDto>> Handle(GetListOfDissertationCohortQuery request,
        CancellationToken cancellationToken)
    {
        var response = new ResponseDto<PaginatedDissertationCohortListDto>();
        this._logger.LogInformation("Attempting to retrieve list of Dissertation Cohort");
        PagedList<Domain.Entities.DissertationCohort> dissertationCohort = this._db.DissertationCohortRepository.GetListOfDissertationCohort(request.Parameters);

        var mappedDissertationCohort = new PagedList<GetDissertationCohort>(
            dissertationCohort.Select(MapToDissertationCohortDto).ToList(),
            dissertationCohort.TotalCount,
            dissertationCohort.CurrentPage,
            dissertationCohort.PageSize
        );

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = new PaginatedDissertationCohortListDto
        {
            Data = mappedDissertationCohort,
            TotalCount = mappedDissertationCohort.TotalCount,
            PageSize = mappedDissertationCohort.PageSize,
            CurrentPage = mappedDissertationCohort.CurrentPage,
            TotalPages = mappedDissertationCohort.TotalPages,
            HasNext = mappedDissertationCohort.HasNext,
            HasPrevious = mappedDissertationCohort.HasPrevious
        };

        return Task.FromResult(response);
    }

    private GetDissertationCohort MapToDissertationCohortDto(
        Domain.Entities.DissertationCohort dissertationCohort)
    {
        GetAcademicYear mappedAcademicYear = this._mapper.Map<GetAcademicYear>(dissertationCohort.AcademicYear);
        return new GetDissertationCohort
        {
            Id = dissertationCohort.Id,
            StartDate = dissertationCohort.StartDate,
            CreatedAt = dissertationCohort.CreatedAt,
            EndDate = dissertationCohort.EndDate,
            CreatedBy = dissertationCohort.CreatedBy,
            UpdatedAt = dissertationCohort.UpdatedAt,
            UpdatedBy = dissertationCohort.UpdatedBy,
            SupervisionChoiceDeadline = dissertationCohort.SupervisionChoiceDeadline,
            AcademicYear = mappedAcademicYear,
            Status = dissertationCohort.StartDate.Date <= DateTime.UtcNow.Date && dissertationCohort.EndDate.Date >= DateTime.UtcNow.Date
                ? DissertationConfigStatus.Active
                : DissertationConfigStatus.InActive
        };
    }
}