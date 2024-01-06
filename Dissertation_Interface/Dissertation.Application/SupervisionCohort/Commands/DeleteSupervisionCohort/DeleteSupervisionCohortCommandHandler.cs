using Dissertation.Domain.Interfaces;
using MediatR;
using Shared.DTO;
using Shared.Logging;

namespace Dissertation.Application.SupervisionCohort.Commands.DeleteSupervisionCohort;

public class DeleteSupervisionCohortCommandHandler : IRequestHandler<DeleteSupervisionCohortCommand, ResponseDto<string>>
{
    private readonly IAppLogger<DeleteSupervisionCohortCommandHandler> _logger;
    private readonly IUserApiService _userApiService;

    public DeleteSupervisionCohortCommandHandler(IAppLogger<DeleteSupervisionCohortCommandHandler> logger, IUserApiService userApiService)
    {
        this._logger = logger;
        this._userApiService = userApiService;
    }

    public async Task<ResponseDto<string>> Handle(DeleteSupervisionCohortCommand request, CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to Remove a Supervisor from a Supervision Cohort {cohortId}", request.SupervisionCohortId);
        ResponseDto<string> userResponse = await this._userApiService.DeleteSupervisionCohort(request.SupervisionCohortId);
        return userResponse;
    }
}