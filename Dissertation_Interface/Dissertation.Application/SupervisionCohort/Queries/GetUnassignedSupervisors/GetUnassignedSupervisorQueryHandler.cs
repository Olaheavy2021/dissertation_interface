using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using MediatR;
using Shared.DTO;
using Shared.Logging;

namespace Dissertation.Application.SupervisionCohort.Queries.GetUnassignedSupervisors;

public class GetUnassignedSupervisorQueryHandler : IRequestHandler<GetUnassignedSupervisorsQuery, ResponseDto<PaginatedUserListDto>>
{
    private readonly IAppLogger<GetUnassignedSupervisorQueryHandler> _logger;
    private readonly IUserApiService _userApiService;
    private readonly IUnitOfWork _db;

    public GetUnassignedSupervisorQueryHandler(IAppLogger<GetUnassignedSupervisorQueryHandler> logger, IUserApiService userApiService, IUnitOfWork db)
    {
        this._logger = logger;
        this._userApiService = userApiService;
        this._db = db;
    }
    public async Task<ResponseDto<PaginatedUserListDto>> Handle(GetUnassignedSupervisorsQuery request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to retrieve list of Unassigned Supervisor");

        //check if the dissertation cohort is passed
        if (request.Parameters.DissertationCohortId == 0)
        {
            Domain.Entities.DissertationCohort? cohort = await this._db.DissertationCohortRepository.GetActiveDissertationCohort();
            if (cohort == null)
            {
                return new ResponseDto<PaginatedUserListDto>()
                {
                    IsSuccess = false,
                    Message = "Kindly filter by the cohort as there is no active cohort."
                };
            }

            request.Parameters.DissertationCohortId = cohort.Id;
        }
        return await this._userApiService.GetUnAssignedSupervisors(request.Parameters);

    }
}