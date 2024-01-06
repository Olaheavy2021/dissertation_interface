using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using MediatR;
using Shared.DTO;
using Shared.Logging;

namespace Dissertation.Application.SupervisionCohort.Commands.CreateSupervisionCohort;

public class CreateSupervisionCohortCommandHandler : IRequestHandler<CreateSupervisionCohortCommand, ResponseDto<string>>
{
    private readonly IAppLogger<CreateSupervisionCohortCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IUserApiService _userApiService;

    public CreateSupervisionCohortCommandHandler(IAppLogger<CreateSupervisionCohortCommandHandler> logger, IUnitOfWork db, IUserApiService userApiService)
    {
        this._logger = logger;
        this._db = db;
        this._userApiService = userApiService;
    }

    public async Task<ResponseDto<string>> Handle(CreateSupervisionCohortCommand request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to Add {count} Supervisors to a Supervision Cohort", request.SupervisionCohortRequests.Count);
        var response = new ResponseDto<string>();

        //get active cohort
        Domain.Entities.DissertationCohort? cohort = await this._db.DissertationCohortRepository.GetActiveDissertationCohort();
        if (cohort == null)
        {
            response.IsSuccess = false;
            response.Message = "Kindly initiate a new and active dissertation cohort before inviting Supervisors";
            return response;
        }

        var userRequest = new CreateSupervisionCohortListRequest
        {
            SupervisionCohortRequests = request.SupervisionCohortRequests,
            DissertationCohortId = cohort.Id
        };
        ResponseDto<string> userResponse = await this._userApiService.CreateSupervisionCohortListRequest(userRequest);

        return userResponse;
    }
}