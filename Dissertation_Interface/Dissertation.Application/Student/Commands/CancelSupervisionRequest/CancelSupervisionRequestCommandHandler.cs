using Dissertation.Application.Utility;
using Dissertation.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.DTO;

namespace Dissertation.Application.Student.Commands.CancelSupervisionRequest;

public class CancelSupervisionRequestCommandHandler : IRequestHandler<CancelSupervisionRequestCommand, ResponseDto<string>>
{
    private readonly ILogger<CancelSupervisionRequestCommandHandler> _logger;
    private readonly IUserApiService _userApiService;
    private readonly IHtmlSanitizerService _htmlSanitizer;

    public CancelSupervisionRequestCommandHandler(ILogger<CancelSupervisionRequestCommandHandler> logger, IUserApiService userApiService, IHtmlSanitizerService htmlSanitizer)
    {
        this._logger = logger;
        this._userApiService = userApiService;
        this._htmlSanitizer = htmlSanitizer;
    }

    public async Task<ResponseDto<string>> Handle(CancelSupervisionRequestCommand request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Processing a Student Cancelling a Supervision Request");
        //sanitize the comment
        var sanitizedComment = this._htmlSanitizer.Sanitize(request.Comment);

        var apiRequest = new ActionSupervisionRequest() { Comment = sanitizedComment, RequestId = request.RequestId };
        ResponseDto<string> apiResponse = await this._userApiService.CancelSupervisionRequest(apiRequest);
        return apiResponse;
    }
}