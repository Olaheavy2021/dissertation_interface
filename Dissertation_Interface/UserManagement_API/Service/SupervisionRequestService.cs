using AutoMapper;
using Microsoft.Extensions.Options;
using Shared.Constants;
using Shared.DTO;
using Shared.Enums;
using Shared.Exceptions;
using Shared.Helpers;
using Shared.MessageBus;
using Shared.Settings;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;
using UserManagement_API.Data.Models.Dto;
using UserManagement_API.Helpers;
using UserManagement_API.Service.IService;

namespace UserManagement_API.Service;

public class SupervisionRequestService : ISupervisionRequestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISupervisionCohortService _supervisionCohortService;
    private readonly IDissertationApiService _dissertationApiService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly ILogger<SupervisionRequestService> _logger;
    private readonly IMessageBus _messageBus;
    private readonly ServiceBusSettings _serviceBusSettings;

    public SupervisionRequestService(IUnitOfWork unitOfWork, ISupervisionCohortService supervisionCohortService,
        IDissertationApiService dissertationApiService, IHttpContextAccessor httpContextAccessor, IMapper mapper,
        ILogger<SupervisionRequestService> logger, IMessageBus messageBus,  IOptions<ServiceBusSettings> serviceBusSettings)
    {
        this._unitOfWork = unitOfWork;
        this._supervisionCohortService = supervisionCohortService;
        this._dissertationApiService = dissertationApiService;
        this._httpContextAccessor = httpContextAccessor;
        this._mapper = mapper;
        this._logger = logger;
        this._messageBus = messageBus;
        this._serviceBusSettings = serviceBusSettings.Value;
    }

    public async Task<ResponseDto<string>> CreateSupervisionRequest(CreateSupervisionRequest request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Initiating a Supervision Request for a Student");
        #region Validation Checks
        #region Cohort Checks
        //get the active cohort
        ResponseDto<GetDissertationCohort> dissertationCohort =
            await this._dissertationApiService.GetActiveDissertationCohort();
        if (dissertationCohort.Result == null)
        {
            return new ResponseDto<string>
            {
                Message = "There is currently no Active Dissertation Cohort",
                Result = ErrorMessages.DefaultError,
                IsSuccess = false
            };
        }

        if (DateTime.UtcNow.Date > dissertationCohort.Result.SupervisionChoiceDeadline.Date)
        {
            return new ResponseDto<string>
            {
                Message = "The supervision request deadline has passed for this dissertation cohort",
                Result = ErrorMessages.DefaultError,
                IsSuccess = false
            };
        }
        #endregion
        #region SupervisionCohort Checks
        //check if the supervision cohort exists and supervision slot has not been exceeded
        var parameters = new SupervisionCohortParameters
        {
            DissertationCohortId = dissertationCohort.Result.Id, SupervisorId = request.SupervisorId
        };

        ResponseDto<SupervisionCohort> supervisionCohort =
            await this._supervisionCohortService.GetSupervisionCohort(parameters);
        if (!supervisionCohort.IsSuccess && supervisionCohort.Result == null)
        {
            return new ResponseDto<string>
            {
                Message = supervisionCohort.Message, IsSuccess = false, Result = ErrorMessages.DefaultError
            };
        }

        //check if the supervisor has any slots remaining
        if (supervisionCohort.Result != null && supervisionCohort.Result.AvailableSupervisionSlot == 0)
        {
            return new ResponseDto<string>()
            {
                Message = "There are no supervision slots available for this supervisor",
                IsSuccess = false,
                Result = ErrorMessages.DefaultError
            };
        }
        #endregion

        #region StudentChecks
        //get the student from the token
        var studentId = this._httpContextAccessor.HttpContext?.Items["UserId"] as string;
        if (string.IsNullOrEmpty(studentId))
        {
            return new ResponseDto<string>
            {
                Message = "Invalid Request", Result = ErrorMessages.DefaultError, IsSuccess = false
            };
        }

        //count the number of active requests the student has
        var activeRequests = await CountNumberOfActiveRequests(studentId, dissertationCohort.Result.Id);
        if (activeRequests >= 3)
        {
            return new ResponseDto<string>
            {
                Message = "You can only have 3 pending requests at a time", Result = ErrorMessages.DefaultError, IsSuccess = false
            };
        }

        var activeRequestWithSupervisor =
            await CheckIfStudentHasAPendingRequestWithThisSupervisor(request.SupervisorId, studentId,
                dissertationCohort.Result.Id);
        if (activeRequestWithSupervisor)
        {
            return new ResponseDto<string>
            {
                Message = "You have a pending request sent to this supervisor", Result = ErrorMessages.DefaultError, IsSuccess = false
            };
        }

        //check if the student has a supervisor already
        var isStudentPairedAlready = await CheckIfStudentHasASupervisor(studentId, dissertationCohort.Result.Id);
        if (isStudentPairedAlready)
        {
            return new ResponseDto<string>
            {
                Message = "This Student is paired to a supervisor already", Result = ErrorMessages.DefaultError, IsSuccess = false
            };
        }
        #endregion
        #region Supervisor Checks
        //check if the supervisor has been disabled
        if (supervisionCohort.Result != null && supervisionCohort.Result.Supervisor.IsLockedOutByAdmin)
        {
            return new ResponseDto<string>()
            {
                Message = "The supervisor is currently disabled by the admin",
                IsSuccess = false,
                Result = ErrorMessages.DefaultError
            };
        }
        #endregion
        #endregion
        var supervisionRequest = SupervisionRequest.Create(
            request.SupervisorId,
            studentId,
            dissertationCohort.Result.Id
        );
        await this._unitOfWork.SupervisionRequestRepository.AddAsync(supervisionRequest);
        await this._unitOfWork.SaveAsync(cancellationToken);
        await PublishSupervisionRequestNotification(supervisionCohort.Result?.Supervisor!,
            EmailType.EmailTypeSupervisionRequestInitiatedEmail);

        return new ResponseDto<string>
        {
            Message = "Supervision Request initiated Successfully",
            Result = SuccessMessages.DefaultSuccess,
            IsSuccess = true
        };
    }

    public async Task<ResponseDto<PaginatedSupervisionRequestListDto>> GetPaginatedListOfSupervisionRequest(
        SupervisionRequestPaginationParameters parameters)
    {
        var response = new ResponseDto<PaginatedSupervisionRequestListDto>();
        PagedList<SupervisionRequest> supervisionRequests =
            this._unitOfWork.SupervisionRequestRepository.GetPaginatedListOfSupervisionRequests(parameters);

        ResponseDto<IReadOnlyList<GetDepartment>> departments = await this._dissertationApiService.GetAllDepartments();
        ResponseDto<IReadOnlyList<GetCourse>> courses = await this._dissertationApiService.GetAllCourses();

        if (departments.Result == null)
        {
            throw new NotFoundException("Departments", "all");
        }

        if (courses.Result == null)
        {
            throw new NotFoundException("Courses", "all");
        }

        var data = new PagedList<SupervisionRequestListDto>(
            supervisionRequests.Select(supervisionRequest =>
                    CustomMappers.MapToSupervisionRequestListDto(supervisionRequest, departments.Result,
                        courses.Result))
                .ToList(),
            supervisionRequests.TotalCount,
            supervisionRequests.CurrentPage,
            supervisionRequests.PageSize
        );

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = new PaginatedSupervisionRequestListDto
        {
            Data = data,
            CurrentPage = supervisionRequests.CurrentPage,
            TotalPages = supervisionRequests.TotalPages,
            HasNext = supervisionRequests.HasNext,
            HasPrevious = supervisionRequests.HasPrevious,
            TotalCount = supervisionRequests.TotalCount,
            PageSize = supervisionRequests.PageSize
        };

        return response;
    }

    public async Task<ResponseDto<PaginatedSupervisionRequestListDto>> GetPaginatedListOfSupervisionRequestForAStudent(
        SupervisionRequestPaginationParameters parameters)
    {
        var response = new ResponseDto<PaginatedSupervisionRequestListDto>();
        PagedList<SupervisionRequest> supervisionRequests =
            this._unitOfWork.SupervisionRequestRepository.GetStudentListOfSupervisionRequests(parameters);

        ResponseDto<IReadOnlyList<GetDepartment>> departments = await this._dissertationApiService.GetAllDepartments();
        ResponseDto<IReadOnlyList<GetCourse>> courses = await this._dissertationApiService.GetAllCourses();

        if (departments.Result == null)
        {
            throw new NotFoundException("Departments", "all");
        }

        if (courses.Result == null)
        {
            throw new NotFoundException("Courses", "all");
        }

        var data = new PagedList<SupervisionRequestListDto>(
            supervisionRequests.Select(supervisionRequest =>
                    CustomMappers.MapToSupervisionRequestListDto(supervisionRequest, departments.Result,
                        courses.Result))
                .ToList(),
            supervisionRequests.TotalCount,
            supervisionRequests.CurrentPage,
            supervisionRequests.PageSize
        );

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = new PaginatedSupervisionRequestListDto
        {
            Data = data,
            CurrentPage = supervisionRequests.CurrentPage,
            TotalPages = supervisionRequests.TotalPages,
            HasNext = supervisionRequests.HasNext,
            HasPrevious = supervisionRequests.HasPrevious,
            TotalCount = supervisionRequests.TotalCount,
            PageSize = supervisionRequests.PageSize
        };

        return response;
    }

    public async Task<ResponseDto<PaginatedSupervisionRequestListDto>>
        GetPaginatedListOfSupervisionRequestForASupervisor(
            SupervisionRequestPaginationParameters parameters)
    {
        var response = new ResponseDto<PaginatedSupervisionRequestListDto>();
        PagedList<SupervisionRequest> supervisionRequests =
            this._unitOfWork.SupervisionRequestRepository.GetSupervisorListOfSupervisionRequests(parameters);

        ResponseDto<IReadOnlyList<GetDepartment>> departments = await this._dissertationApiService.GetAllDepartments();
        ResponseDto<IReadOnlyList<GetCourse>> courses = await this._dissertationApiService.GetAllCourses();

        if (departments.Result == null)
        {
            throw new NotFoundException("Departments", "all");
        }

        if (courses.Result == null)
        {
            throw new NotFoundException("Courses", "all");
        }

        var data = new PagedList<SupervisionRequestListDto>(
            supervisionRequests.Select(supervisionRequest =>
                    CustomMappers.MapToSupervisionRequestListDto(supervisionRequest, departments.Result,
                        courses.Result))
                .ToList(),
            supervisionRequests.TotalCount,
            supervisionRequests.CurrentPage,
            supervisionRequests.PageSize
        );

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = new PaginatedSupervisionRequestListDto
        {
            Data = data,
            CurrentPage = supervisionRequests.CurrentPage,
            TotalPages = supervisionRequests.TotalPages,
            HasNext = supervisionRequests.HasNext,
            HasPrevious = supervisionRequests.HasPrevious,
            TotalCount = supervisionRequests.TotalCount,
            PageSize = supervisionRequests.PageSize
        };

        return response;
    }

    public async Task<ResponseDto<string>> RejectSupervisionRequest(ActionSupervisionRequest request,
        CancellationToken cancellationToken)
    {
        //fetch the supervision request
        SupervisionRequest supervisionRequest = await GetSupervisionRequest(request.RequestId);

        //check that supervision request matches the supervisor
        //get the supervisor from the token
        var supervisorId = GetLoggedInUserId();
        if (!string.IsNullOrEmpty(supervisorId) && !supervisorId.Equals(supervisionRequest.SupervisorId))
        {
            return new ResponseDto<string>()
            {
                Message = "You are not authorized to action this request", IsSuccess = false,
            };
        }

        //check that the request is in a pending state
        if (supervisionRequest.Status != SupervisionRequestStatus.Pending)
        {
            return new ResponseDto<string>()
            {
                Message = "Supervision Request is not in the Pending State", IsSuccess = false,
            };
        }

        //change the status
        supervisionRequest.Status = SupervisionRequestStatus.Rejected;
        supervisionRequest.Comment = request.Comment;
        this._unitOfWork.SupervisionRequestRepository.Update(supervisionRequest);

        //save changes
        await this._unitOfWork.SaveAsync(cancellationToken);
        //Send an email to the student that there is a response
        await PublishSupervisionRequestNotification(supervisionRequest.Student,
            EmailType.EmailTypeSupervisionRequestActionedEmail);

        return new ResponseDto<string>()
        {
            Message = "Supervision Request rejected successfully",
            IsSuccess = true,
            Result = SuccessMessages.DefaultSuccess
        };
    }

    public async Task<ResponseDto<string>> AcceptSupervisionRequest(ActionSupervisionRequest request,
        CancellationToken cancellationToken)
    {
        //fetch the supervision request
        SupervisionRequest supervisionRequest = await GetSupervisionRequest(request.RequestId);
        SupervisionCohort supervisionCohort =
            await GetSupervisionCohort(supervisionRequest.SupervisorId, supervisionRequest.DissertationCohortId);

        //check that supervision request matches the supervisor
        //get the supervisor from the token
        var supervisorId = GetLoggedInUserId();
        if (!string.IsNullOrEmpty(supervisorId) && !supervisorId.Equals(supervisionRequest.SupervisorId))
        {
            return new ResponseDto<string>
            {
                Message = "You are not authorized to action this request", IsSuccess = false,
            };
        }

        //check that the request is in a pending state
        if (supervisionRequest.Status != SupervisionRequestStatus.Pending)
        {
            return new ResponseDto<string>()
            {
                Message = "Supervision Request is not in the Pending State", IsSuccess = false,
            };
        }

        //check the availability slot for the supervisor
        if (supervisionCohort.AvailableSupervisionSlot == 0)
        {
            return new ResponseDto<string>
            {
                Message = "You have reached the maximum supervision slots allocated to you", IsSuccess = false,
            };
        }

        //change the status
        supervisionRequest.Status = SupervisionRequestStatus.Approved;
        supervisionRequest.Comment = request.Comment;
        this._unitOfWork.SupervisionRequestRepository.Update(supervisionRequest);

        //insert a record into the supervision list table
        var supervisionList = SupervisionList.Create(
            supervisorId,
            supervisionRequest.StudentId,
            supervisionRequest.DissertationCohortId
        );
        await this._unitOfWork.SupervisionListRepository.AddAsync(supervisionList);

        //reduce the available supervision slot for the supervisor
        supervisionCohort.AvailableSupervisionSlot -= 1;
        this._unitOfWork.SupervisionCohortRepository.Update(supervisionCohort);

        //save all changes
        await this._unitOfWork.SaveAsync(cancellationToken);

        //Send an email to the student that there is a response
        await PublishSupervisionRequestNotification(supervisionRequest.Student,
            EmailType.EmailTypeSupervisionRequestActionedEmail);

        return new ResponseDto<string>
        {
            Message = "Supervision Request accepted successfully",
            IsSuccess = true,
            Result = SuccessMessages.DefaultSuccess
        };
    }

    public async Task<ResponseDto<string>> CancelSupervisionRequest(ActionSupervisionRequest request,
        CancellationToken cancellationToken)
    {
        //fetch the supervision request
        SupervisionRequest supervisionRequest = await GetSupervisionRequest(request.RequestId);

        //check that supervision request matches the student
        //get the student from the token
        var studentId = GetLoggedInUserId();
        if (!string.IsNullOrEmpty(studentId) && !studentId.Equals(supervisionRequest.StudentId))
        {
            return new ResponseDto<string>()
            {
                Message = "You are not authorized to action this request", IsSuccess = false,
            };
        }

        //check that the request is in a pending state
        if (supervisionRequest.Status != SupervisionRequestStatus.Pending)
        {
            return new ResponseDto<string>()
            {
                Message = "Supervision Request is not in the Pending State", IsSuccess = false,
            };
        }

        //delete the request
        this._unitOfWork.SupervisionRequestRepository.Remove(supervisionRequest);
        await this._unitOfWork.SaveAsync(cancellationToken);

        return new ResponseDto<string>()
        {
            Message = "Supervision Request cancelled successfully",
            IsSuccess = true,
            Result = SuccessMessages.DefaultSuccess
        };
    }

    private async Task PublishSupervisionRequestNotification(ApplicationUser user, string emailType)
    {
        UserDto? userToReturn = this._mapper.Map<UserDto>(user);
        this._logger.LogInformation("Supervision Request Email Published for this user {0}",
            user.UserName ?? string.Empty);

        var emailDto = new PublishEmailDto { User = userToReturn, EmailType = emailType };
        await this._messageBus.PublishMessage(emailDto, this._serviceBusSettings.EmailLoggerQueue,
            this._serviceBusSettings.ServiceBusConnectionString);
    }

    private async Task<SupervisionRequest> GetSupervisionRequest(long requestId)
    {
        SupervisionRequest? supervisionRequest =
            await this._unitOfWork.SupervisionRequestRepository.GetFirstOrDefaultAsync(x => x.Id == requestId,
                includes: x => x.Student);
        if (supervisionRequest == null)
        {
            throw new NotFoundException(nameof(supervisionRequest), requestId);
        }

        return supervisionRequest;
    }

    private async Task<SupervisionCohort> GetSupervisionCohort(string supervisorId, long cohortId )
    {
        SupervisionCohort? supervisionCohort =
            await this._unitOfWork.SupervisionCohortRepository.GetFirstOrDefaultAsync(x => x.DissertationCohortId == cohortId && x.SupervisorId == supervisorId);
        if (supervisionCohort == null)
        {
            throw new NotFoundException(nameof(SupervisionCohort), supervisorId);
        }
        return supervisionCohort;
    }

    private async Task<int> CountNumberOfActiveRequests(string studentId, long cohortId) =>
        await this._unitOfWork.SupervisionRequestRepository.CountWhere(x =>
            x.StudentId == studentId && x.DissertationCohortId == cohortId && x.Status == SupervisionRequestStatus.Pending);

    private async Task<bool> CheckIfStudentHasASupervisor(string studentId, long cohortId)
    {
        var doesStudentHaveASupervisor =  await this._unitOfWork.SupervisionListRepository.AnyAsync(x =>
            x.StudentId == studentId && x.DissertationCohortId == cohortId);
        return doesStudentHaveASupervisor;
    }

    private async Task<bool> CheckIfStudentHasAPendingRequestWithThisSupervisor(string supervisorId, string studentId, long cohortId) =>
        await this._unitOfWork.SupervisionRequestRepository.AnyAsync(x =>
            x.StudentId == studentId && x.SupervisorId == supervisorId && x.Status == SupervisionRequestStatus.Pending && x.DissertationCohortId == cohortId);

    private string GetLoggedInUserId()
    {
        var userId = this._httpContextAccessor.HttpContext?.Items["UserId"] as string;
        if (string.IsNullOrEmpty(userId))
        {
            throw new NotFoundException("UserId", "AccessToken");
        }

        return userId;
    }

}