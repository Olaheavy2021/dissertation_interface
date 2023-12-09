using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Enums;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.DissertationCohort.Queries.GetActiveDissertationCohort;

public class GetActiveDissertationCohortQueryHandler : IRequestHandler<GetActiveDissertationCohortQuery, ResponseDto<GetDissertationCohort>>
{
    private readonly IAppLogger<GetActiveDissertationCohortQueryHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public GetActiveDissertationCohortQueryHandler(IAppLogger<GetActiveDissertationCohortQueryHandler> logger, IUnitOfWork db, IMapper mapper)
    {
        this._logger = logger;
        this._db = db;
        this._mapper = mapper;
    }
    public async Task<ResponseDto<GetDissertationCohort>> Handle(GetActiveDissertationCohortQuery request,
        CancellationToken cancellationToken)
    {
        var response = new ResponseDto<GetDissertationCohort>();
        this._logger.LogInformation("Attempting to retrieve the active Dissertation Cohort");
        Domain.Entities.DissertationCohort? dissertationCohort =
            await this._db.DissertationCohortRepository.GetActiveDissertationCohort();

        if (dissertationCohort is null)
        {
            this._logger.LogError("No active Dissertation Cohort found");
            throw new NotFoundException(nameof(DissertationCohort), DissertationConfigStatus.Active);
        }

        GetDissertationCohort mappedDissertationCohort = this._mapper.Map<GetDissertationCohort>(dissertationCohort);
        mappedDissertationCohort.UpdateStatus();

        this._logger.LogInformation("Successfully retrieved the active Dissertation Cohort");
        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedDissertationCohort;
        return response;
    }
}