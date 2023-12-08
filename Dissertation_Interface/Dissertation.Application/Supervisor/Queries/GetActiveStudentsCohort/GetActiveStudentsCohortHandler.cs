using Dissertation.Domain.Enums;
using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.Supervisor.Queries.GetActiveStudentsCohort;

public class GetActiveStudentsCohortHandler : IRequestHandler<GetActiveStudentsCohortQuery, ResponseDto<PaginatedUserListDto>>
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

    public async Task<ResponseDto<PaginatedUserListDto>> Handle(GetActiveStudentsCohortQuery request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to retrieve list of students");

        // Determine the cohort based on the condition
        Domain.Entities.DissertationCohort? cohort = await this._db.DissertationCohortRepository.GetActiveDissertationCohort();

        if (cohort == null)
        {
            throw new NotFoundException(nameof(DissertationCohort), DissertationConfigStatus.Active);
        }

        // Map the request parameters to the pagination parameters
        DissertationStudentPaginationParameters parameters = this._mapper.Map<DissertationStudentPaginationParameters>(request.Parameters);
        parameters.CohortEndDate = cohort.EndDate;
        parameters.CohortStartDate = cohort.StartDate;

        // Fetch the list of students
        ResponseDto<PaginatedUserListDto> students = await this._userApiService.GetListOfStudents(parameters);
        return students;
    }
}