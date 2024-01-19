using System.Text.Json;
using Dissertation.Application.DissertationCohort.Queries.GetActiveDissertationCohort;
using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Logging;

namespace Dissertation.Application.SupervisorSuggestion.Commands.InitiateMatching;

public class InitiateMatchingCommandHandler : IRequestHandler<InitiateMatchingCommand, ResponseDto<string>>
{
    private readonly IUserApiService _userApiService;
    private readonly IDissertationMatchingService _dissertationMatchingService;
    private readonly ISender _sender;
    private readonly IAppLogger<InitiateMatchingCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public InitiateMatchingCommandHandler(IUserApiService userApiService, IAppLogger<InitiateMatchingCommandHandler> logger, ISender sender, IUnitOfWork unitOfWork, IMapper mapper, IDissertationMatchingService dissertationMatchingService)
    {
        this._userApiService = userApiService;
        this._logger = logger;
        this._sender = sender;
        this._unitOfWork = unitOfWork;
        this._mapper = mapper;
        this._dissertationMatchingService = dissertationMatchingService;
    }

    public async Task<ResponseDto<string>> Handle(InitiateMatchingCommand request, CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Initiating matching process.");

        //fetch the active dissertation cohort
        var query = new GetActiveDissertationCohortQuery();
        ResponseDto<GetDissertationCohort> activeDissertationCohort = await this._sender.Send(query, cancellationToken);
        if (!activeDissertationCohort.IsSuccess || activeDissertationCohort.Result == null)
        {
            this._logger.LogWarning("No active dissertation cohort found.");
            return ResponseWithError("There is no active dissertation cohort");
        }

        this._logger.LogInformation($"Active dissertation cohort found with ID: {activeDissertationCohort.Result.Id}");

        //fetch the list of students for this cohort
        IReadOnlyList<Domain.Entities.Student> students =
            await this._unitOfWork.StudentRepository.GetAllAsync(x =>
                x.DissertationCohortId == activeDissertationCohort.Result.Id);

        if (!students.Any())
        {
            return ResponseWithError("Students have not been added to the dissertation cohort");
        }

        var filteredStudents = students.Where(student => !string.IsNullOrEmpty(student.ResearchTopic)).ToList();
        List<StudentMatchingRequest> studentRequest = this._mapper.Map<List<StudentMatchingRequest>>(filteredStudents);

        //fetch the list of supervisors available for this cohort
        ResponseDto<List<GetSupervisionCohort>> supervisorsAddedToCohort = await this._userApiService.GetAllSupervisionCohort(activeDissertationCohort.Result.Id);
        if (!supervisorsAddedToCohort.IsSuccess || supervisorsAddedToCohort.Result == null)
        {
            return ResponseWithError("Supervisors have not been added to the dissertation cohort");
        }

        IReadOnlyList<Domain.Entities.Supervisor> supervisors = await this._unitOfWork.SupervisorRepository.GetAllAsync();
        if (!supervisors.Any())
        {
            return ResponseWithError("Supervisors have not been added to the dissertation cohort");
        }

        var filteredSupervisors = supervisors.Where(supervisor => !string.IsNullOrEmpty(supervisor.ResearchArea)).ToList();
        List<SupervisorMatchingRequest> supervisorRequest = MatchSupervisorsToCohorts(filteredSupervisors, supervisorsAddedToCohort.Result);

        var initiatingMatchingRequest = new InitiateMatchingRequest
        {
            Supervisor = supervisorRequest,
            Student = studentRequest
        };

        InitiateMatchingResponse initiatingMatchingResponse = await this._dissertationMatchingService.ProcessData(initiatingMatchingRequest);
        this._logger.LogInformation(JsonSerializer.Serialize(initiatingMatchingRequest));
        return new ResponseDto<string>
        {
            Message = $"{initiatingMatchingRequest.Supervisor.Count} Supervisors and {initiatingMatchingRequest.Student.Count} Students data are currently being processed.",
            IsSuccess = true,
            Result = initiatingMatchingResponse.Task
        };
    }

    private List<SupervisorMatchingRequest> MatchSupervisorsToCohorts(IReadOnlyCollection<Domain.Entities.Supervisor> supervisors,
        IReadOnlyCollection<GetSupervisionCohort> cohorts)
    {
        var matchingRequests = new List<SupervisorMatchingRequest>();

        foreach (Domain.Entities.Supervisor supervisor in supervisors)
        {
            // Find the corresponding cohort
            GetSupervisionCohort? cohort = cohorts.FirstOrDefault(c => c.UserDetails.Id == supervisor.UserId);

            if (cohort != null)
            {
                var matchingRequest = new SupervisorMatchingRequest
                {
                    ResearchArea = supervisor.ResearchArea ?? string.Empty,
                    AvailableSlot = cohort.AvailableSupervisionSlot,
                    Id = supervisor.UserId
                };

                matchingRequests.Add(matchingRequest);
            }
        }

        return matchingRequests;
    }

    private ResponseDto<string> ResponseWithError(string message)
    {
        this._logger.LogWarning(message);
        return new ResponseDto<string>
        {
            IsSuccess = false,
            Message = message,
            Result = ErrorMessages.DefaultError
        };
    }
}