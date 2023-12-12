using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.DTO;
using Shared.Enums;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.Supervisor.Queries.GetActiveStudentsCohort;

public class GetActiveStudentsCohortHandler : IRequestHandler<GetActiveStudentsCohortQuery, ResponseDto<PaginatedStudentListDto>>
{
    private readonly IAppLogger<GetActiveStudentsCohortQuery> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;
    private readonly IUserApiService _userApiService;

    public GetActiveStudentsCohortHandler(IAppLogger<GetActiveStudentsCohortQuery> logger, IUnitOfWork db, IMapper mapper, IUserApiService userApiService)
    {
        this._logger = logger;
        this._db = db;
        this._mapper = mapper;
        this._userApiService = userApiService;
    }

    public async Task<ResponseDto<PaginatedStudentListDto>> Handle(GetActiveStudentsCohortQuery request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to retrieve list of students");

        Domain.Entities.DissertationCohort? cohort;
        if (request.Parameters.FilterByCohort == 0)
        {
            // Determine the cohort based on the condition
            cohort = await this._db.DissertationCohortRepository.GetActiveDissertationCohort();
        }
        else
        {
            cohort = await this._db.DissertationCohortRepository.GetFirstOrDefaultAsync(x =>
                x.Id == request.Parameters.FilterByCohort);
        }

        if (cohort == null)
        {
            return new ResponseDto<PaginatedStudentListDto>()
            {
                Message = "Kindly filter by a dissertation cohort as there is no active cohort",
                IsSuccess = false
            };
        }

        // Map the request parameters to the pagination parameters
        DissertationStudentPaginationParameters parameters = this._mapper.Map<DissertationStudentPaginationParameters>(request.Parameters);
        parameters.CohortEndDate = cohort.EndDate;
        parameters.CohortStartDate = cohort.StartDate;

        // Fetch the list of students
        ResponseDto<PaginatedStudentListDto> students = await this._userApiService.GetListOfStudents(parameters);
        return students;
    }
}