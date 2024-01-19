using Dissertation.Application.DTO.Response;
using Dissertation.Application.Student.Queries.GetStudentById;
using Dissertation.Application.Supervisor.Queries.GetSupervisorById;
using Dissertation.Infrastructure.Persistence.IRepository;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.SupervisorSuggestion.Queries.GetSuggestionsForStudent;

public class GetSuggestionsForStudentsQueryHandler : IRequestHandler<GetSuggestionsForStudentsQuery, ResponseDto<List<GetSupervisorSuggestion>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppLogger<GetSuggestionsForStudentsQueryHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISender _sender;

    public GetSuggestionsForStudentsQueryHandler(IUnitOfWork unitOfWork, IAppLogger<GetSuggestionsForStudentsQueryHandler> logger, IHttpContextAccessor httpContextAccessor, ISender sender)
    {
        this._unitOfWork = unitOfWork;
        this._logger = logger;
        this._httpContextAccessor = httpContextAccessor;
        this._sender = sender;
    }

    public async Task<ResponseDto<List<GetSupervisorSuggestion>>> Handle(GetSuggestionsForStudentsQuery request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Handling GetSuggestionsForStudentsQuery");

        //get the userId from the httpContext
        var userId = this._httpContextAccessor.HttpContext?.Items["UserId"] as string;
        if (string.IsNullOrEmpty(userId))
        {
            this._logger.LogWarning("User ID not found in HttpContext");
            throw new InvalidOperationException("User ID is required");
        }

        //fetch the student details
        var getStudentByIdQuery = new GetStudentByIdQuery(userId);
        ResponseDto<GetStudent> student = await this._sender.Send(getStudentByIdQuery, cancellationToken);

        if (!student.IsSuccess || student.Result?.StudentDetails == null)
        {
            this._logger.LogWarning($"Student not found for ID {userId}");
            throw new NotFoundException(nameof(Domain.Entities.Student), userId);
        }

        this._logger.LogInformation($"Fetching supervisor suggestions for student ID {student.Result.StudentDetails.Id}");
        IReadOnlyList<Domain.Entities.SupervisorSuggestion> supervisorSuggestions =
            await this._unitOfWork.SupervisorSuggestionRepository.GetAllAsync(x =>
                x.StudentId == student.Result.StudentDetails.Id);

        if (!supervisorSuggestions.Any())
        {
            this._logger.LogWarning("There are no supervisor suggestions for this student - ");
            return new ResponseDto<List<GetSupervisorSuggestion>>()
            {
                IsSuccess = false,
                Message = "There are no suggestions for you at the moment",
            };
        }


        var resultList = new List<GetSupervisorSuggestion>();
        foreach (Domain.Entities.SupervisorSuggestion supervisorSuggestion in supervisorSuggestions)
        {
            var getSupervisorByIdQuery = new GetSupervisorByIdQuery(supervisorSuggestion.SupervisorId);
            ResponseDto<GetSupervisor> supervisor = await this._sender.Send(getSupervisorByIdQuery, cancellationToken);

            if (!supervisor.IsSuccess || supervisor.Result?.SupervisorDetails == null)
            {
                this._logger.LogWarning("Failed to fetch supervisors details - {userId}", supervisorSuggestion.SupervisorId);
                continue;
            }

            resultList.Add(new GetSupervisorSuggestion
            {
                CompatibilityScore = supervisorSuggestion.CompatibilityScore,
                Supervisor = supervisor.Result
            });
        }

        this._logger.LogInformation($"Successfully processed {resultList.Count} supervisor suggestions");
        return new ResponseDto<List<GetSupervisorSuggestion>>()
        {
            Message = SuccessMessages.DefaultSuccess,
            Result = resultList,
            IsSuccess = true
        };
    }
}