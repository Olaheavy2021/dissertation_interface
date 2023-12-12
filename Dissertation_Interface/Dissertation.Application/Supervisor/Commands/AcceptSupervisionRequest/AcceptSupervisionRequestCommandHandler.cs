using Dissertation.Application.Utility;
using Dissertation.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.DTO;

namespace Dissertation.Application.Supervisor.Commands.AcceptSupervisionRequest;

public class AcceptSupervisionRequestCommandHandler : IRequestHandler<AcceptSupervisionRequestCommand, ResponseDto<string>>
{
    private readonly ILogger<AcceptSupervisionRequestCommandHandler> _logger;
    private readonly IUserApiService _userApiService;
    private readonly IHtmlSanitizerService _htmlSanitizer;

    public AcceptSupervisionRequestCommandHandler(ILogger<AcceptSupervisionRequestCommandHandler> logger, IUserApiService userApiService, IHtmlSanitizerService htmlSanitizer)
    {
        this._logger = logger;
        this._userApiService = userApiService;
        this._htmlSanitizer = htmlSanitizer;
    }

    public async Task<ResponseDto<string>> Handle(AcceptSupervisionRequestCommand request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Processing a Supervisor Accepting a Supervision Request");
        //sanitize the comment
        var sanitizedComment = this._htmlSanitizer.Sanitize(request.Comment);

        var apiRequest = new ActionSupervisionRequest() { Comment = sanitizedComment, RequestId = request.RequestId };
        ResponseDto<string> apiResponse = await this._userApiService.AcceptSupervisionRequest(apiRequest);
        return apiResponse;
    }
}