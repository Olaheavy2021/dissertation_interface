using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Helpers;
using Shared.Logging;

namespace Dissertation.Application.Student.Queries.GetListOfStudents;

public class GetListOfStudentsQueryHandler : IRequestHandler<GetListOfStudentsQuery, ResponseDto<PaginatedStudentListDto>>
{
    private readonly IAppLogger<GetListOfStudentsQueryHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;
    private readonly IUserApiService _userApiService;

    public GetListOfStudentsQueryHandler(IAppLogger<GetListOfStudentsQueryHandler> logger, IUnitOfWork db, IMapper mapper, IUserApiService userApiService)
    {
        this._logger = logger;
        this._db = db;
        this._mapper = mapper;
        this._userApiService = userApiService;
    }


    public async Task<ResponseDto<PaginatedStudentListDto>> Handle(GetListOfStudentsQuery request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to retrieve list of students");

        // Determine the cohort based on the condition
        Domain.Entities.DissertationCohort? cohort;
        if (request.Parameters.FilterByCohort > 0)
        {
            cohort = await this._db.DissertationCohortRepository.GetFirstOrDefaultAsync(x => x.Id == request.Parameters.FilterByCohort);
        }
        else
        {
            cohort = await this._db.DissertationCohortRepository.GetActiveDissertationCohort();
        }

        if (cohort == null)
        {
            return new ResponseDto<PaginatedStudentListDto>()
            {
                Message = "Please select a dissertation cohort. There is no active cohort",
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