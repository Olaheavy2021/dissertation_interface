using Dissertation.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.DTO;

namespace Dissertation.Application.Student.Commands.InitiateSupervisionRequest;

public class InitiateSupervisionRequestCommandHandler : IRequestHandler<InitiateSupervisionRequestCommand, ResponseDto<string>>
{
    private readonly IUserApiService _userApiService;
    private readonly ILogger<InitiateSupervisionRequestCommandHandler> _logger;
    public InitiateSupervisionRequestCommandHandler(IUserApiService userApiService, ILogger<InitiateSupervisionRequestCommandHandler> logger)
    {
        this._userApiService = userApiService;
        this._logger = logger;
    }

    public async Task<ResponseDto<string>> Handle(InitiateSupervisionRequestCommand request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Initiating Supervision Request for a student");
        var apiRequest = new CreateSupervisionRequest { SupervisorId = request.SupervisorId };
        return await this._userApiService.CreateSupervisionRequest(apiRequest);
    }
}