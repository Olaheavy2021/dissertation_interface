using Dissertation.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.DTO;

namespace Dissertation.Application.SupervisionCohort.Commands.UpdateSupervisionSlot;

public class UpdateSupervisionSlotCommandHandler : IRequestHandler<UpdateSupervisionSlotCommand, ResponseDto<string>>
{
    private readonly IUserApiService _userApiService;
    private readonly ILogger<UpdateSupervisionSlotCommandHandler> _logger;
    public UpdateSupervisionSlotCommandHandler(IUserApiService userApiService, ILogger<UpdateSupervisionSlotCommandHandler> logger)
    {
        this._userApiService = userApiService;
        this._logger = logger;
    }
    
    public Task<ResponseDto<string>> Handle(UpdateSupervisionSlotCommand request, CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to update a supervision slot for a supervision cohort");
        var apiRequest = new UpdateSupervisionCohortRequest
        {
            SupervisionCohortId = request.SupervisionCohortId, SupervisionSlots = request.SupervisionSlots
        };
        return this._userApiService.UpdateSupervisionSlots(apiRequest);
    }
}