using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.DissertationCohort.Queries.GetById;

public class GetDissertationCohortByIdQueryHandler: IRequestHandler<GetDissertationCohortByIdQuery, ResponseDto<GetDissertationCohort>>
{
    private readonly IAppLogger<GetDissertationCohortByIdQueryHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public GetDissertationCohortByIdQueryHandler(IAppLogger<GetDissertationCohortByIdQueryHandler> logger,IUnitOfWork db, IMapper mapper)
    {
        this._mapper = mapper;
        this._logger = logger;
        this._db = db;
    }

    public async Task<ResponseDto<GetDissertationCohort>> Handle(GetDissertationCohortByIdQuery request,
        CancellationToken cancellationToken)
    {
        var response = new ResponseDto<GetDissertationCohort>();
        this._logger.LogInformation("Attempting to retrieve an Dissertation Cohort by ID {DissertationCohortID}", request.DissertationCohortId);
        Domain.Entities.DissertationCohort? dissertationCohort = await this._db.DissertationCohortRepository.GetAsync(a => a.Id == request.DissertationCohortId, null, a => a.AcademicYear);
        this._logger.LogInformation("This is the dissertation cohort {@DissertationCohort}", dissertationCohort);
        if (dissertationCohort is null)
        {
            this._logger.LogError("No Dissertation Cohort found with ID");
            throw new NotFoundException(nameof(GetDissertationCohort), request.DissertationCohortId);
        }

        GetDissertationCohort mappedDissertationCohort = this._mapper.Map<GetDissertationCohort>(dissertationCohort);
        mappedDissertationCohort.UpdateStatus();

        this._logger.LogInformation("Successfully retrieved an DissertationCohort by ID {DissertationCohort}", dissertationCohort.Id);
        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedDissertationCohort;
        return response;
    }
}