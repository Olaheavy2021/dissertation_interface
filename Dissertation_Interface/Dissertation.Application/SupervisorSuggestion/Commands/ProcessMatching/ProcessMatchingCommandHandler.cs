using Dissertation.Application.SupervisorSuggestion.Commands.InitiateMatching;
using Dissertation.Application.Utility;
using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using MediatR;
using Shared.DTO;
using Shared.Logging;

namespace Dissertation.Application.SupervisorSuggestion.Commands.ProcessMatching;

public class ProcessMatchingCommandHandler : IRequestHandler<ProcessMatchingCommand,InitiateMatchingResponse>
{
    private readonly IDissertationMatchingService _dissertationMatchingService;
    private readonly IAppLogger<InitiateMatchingCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHtmlSanitizerService _htmlSanitizer;

    public ProcessMatchingCommandHandler(IDissertationMatchingService dissertationMatchingService, IAppLogger<InitiateMatchingCommandHandler> logger, IUnitOfWork unitOfWork, IHtmlSanitizerService htmlSanitizer)
    {
        this._dissertationMatchingService = dissertationMatchingService;
        this._logger = logger;
        this._unitOfWork = unitOfWork;
        this._htmlSanitizer = htmlSanitizer;
    }

    public async Task<InitiateMatchingResponse> Handle(ProcessMatchingCommand request, CancellationToken cancellationToken)
    {
        this._logger.LogInformation($"Starting ProcessMatchingCommandHandler for TaskId: {request.TaskId}");

        //check if there is any data and remove it
        var isTableEmpty = await this._unitOfWork.SupervisorSuggestionRepository.IsSupervisorSuggestionTableEmptyAsync();
        if (!isTableEmpty)
        {
            this._logger.LogInformation("Clearing the Table as there is data in it");
            await this._unitOfWork.SupervisorSuggestionRepository.DeleteAllRecords();
        }

        MatchingStatusRootObject matchingStatusResponse =
            await this._dissertationMatchingService.CheckStatus(request.TaskId);

        if (matchingStatusResponse.State.Equals("SUCCESS"))
        {
            if (matchingStatusResponse.Result.Any())
            {
                this._logger.LogInformation("Matching status check successful.");

                var supervisorSuggestions = new List<Domain.Entities.SupervisorSuggestion>();
                foreach (KeyValuePair<string, MatchingStatusStudentInfo> entry in matchingStatusResponse.Result)
                {
                    MatchingStatusStudentInfo studentInfo = entry.Value;
                    var studentId = long.Parse(entry.Key);

                    if (studentInfo.SupervisorSuggestions.Any())
                    {
                        var suggestions = studentInfo.SupervisorSuggestions
                            .Select(suggestion => Domain.Entities.SupervisorSuggestion.Create(
                                suggestion.AvailableSlot,
                                suggestion.CompatibilityScore,
                               this._htmlSanitizer.Sanitize(suggestion.ResearchArea),
                                suggestion.SupervisorId,
                                this._htmlSanitizer.Sanitize(studentInfo.StudentTopic),
                                studentId))
                            .ToList();

                        supervisorSuggestions.AddRange(suggestions);
                    }
                }

                if (supervisorSuggestions.Any())
                {
                    this._logger.LogInformation($"Adding {supervisorSuggestions.Count} supervisor suggestions to the database.");
                    await this._unitOfWork.SupervisorSuggestionRepository.AddRangeAsync(supervisorSuggestions);
                    await this._unitOfWork.SaveAsync(cancellationToken);
                    this._logger.LogInformation("Supervisor suggestions added successfully.");
                }
            }
            else
            {
                this._logger.LogInformation("No supervisor suggestions found to process.");
            }
        }
        else
        {
            this._logger.LogWarning($"Matching status check returned state: {matchingStatusResponse.State}");
        }

        this._logger.LogInformation($"Completed ProcessMatchingCommandHandler for TaskId: {request.TaskId}");
        return new InitiateMatchingResponse { Task = request.TaskId };
    }
}