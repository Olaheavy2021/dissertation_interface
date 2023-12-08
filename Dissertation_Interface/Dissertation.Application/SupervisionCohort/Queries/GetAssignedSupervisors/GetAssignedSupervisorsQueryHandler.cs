using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using MediatR;
using Shared.DTO;
using Shared.Logging;

namespace Dissertation.Application.SupervisionCohort.Queries.GetAssignedSupervisors;

public class GetAssignedSupervisorsQueryHandler : IRequestHandler<GetAssignedSupervisorsQuery, ResponseDto<PaginatedSupervisionCohortListDto>>
{
    private readonly IAppLogger<GetAssignedSupervisorsQueryHandler> _logger;
    private readonly IUserApiService _userApiService;
    private readonly IUnitOfWork _db;

    public GetAssignedSupervisorsQueryHandler(IAppLogger<GetAssignedSupervisorsQueryHandler> logger, IUserApiService userApiService, IUnitOfWork db)
    {
        this._logger = logger;
        this._userApiService = userApiService;
        this._db = db;
    }

    public async Task<ResponseDto<PaginatedSupervisionCohortListDto>> Handle(GetAssignedSupervisorsQuery request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to retrieve list of Unassigned Supervisor");

        //check if the dissertation cohort is passed
        if (request.Parameters.DissertationCohortId == 0)
        {
            Domain.Entities.DissertationCohort? cohort = await this._db.DissertationCohortRepository.GetActiveDissertationCohort();
            if (cohort == null)
            {
                return new ResponseDto<PaginatedSupervisionCohortListDto>()
                {
                    IsSuccess = false,
                    Message = "Kindly initiate a new and active dissertation cohort before inviting Supervisors"
                };
            }

            request.Parameters.DissertationCohortId = cohort.Id;
        }
        return await this._userApiService.GetSupervisionCohorts(request.Parameters);
    }
}