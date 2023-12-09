using Shared.Constants;
using Shared.DTO;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;
using UserManagement_API.Data.Models.Dto;
using UserManagement_API.Service.IService;

namespace UserManagement_API.Service;

public class SupervisionRequestService : ISupervisionRequestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISupervisionCohortService _supervisionCohortService;
    private readonly IDissertationApiService _dissertationApiService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public SupervisionRequestService(IUnitOfWork unitOfWork, ISupervisionCohortService supervisionCohortService, IDissertationApiService dissertationApiService, IHttpContextAccessor httpContextAccessor)
    {
        this._unitOfWork = unitOfWork;
        this._supervisionCohortService = supervisionCohortService;
        this._dissertationApiService = dissertationApiService;
        this._httpContextAccessor = httpContextAccessor;
    }

    public async Task<ResponseDto<string>> CreateSupervisionRequest(CreateSupervisionRequest request, CancellationToken cancellationToken)
    {
        #region Validation Checks
        //get the student from the token
       var studentId = this._httpContextAccessor.HttpContext?.Items["UserId"] as string;
       if (string.IsNullOrEmpty(studentId))
       {
           return new ResponseDto<string>
           {
               Message = "Invalid Request",
               Result = ErrorMessages.DefaultError,
               IsSuccess = false
           };
       }

        //get the active cohort
        ResponseDto<GetDissertationCohort> dissertationCohort = await this._dissertationApiService.GetActiveDissertationCohort();
        if (dissertationCohort.Result == null)
        {
            return new ResponseDto<string>
            {
                Message = "There is currently no Active Dissertation Cohort",
                Result = ErrorMessages.DefaultError,
                IsSuccess = false
            };
        }

        //check if the supervision cohort exists and supervision slot has not been exceeded
        var parameters = new SupervisionCohortParameters
        {
            DissertationCohortId = dissertationCohort.Result.Id, SupervisorId = request.SupervisorId
        };

        ResponseDto<SupervisionCohort> supervisionCohort = await this._supervisionCohortService.GetSupervisionCohort(parameters);
        if (!supervisionCohort.IsSuccess)
        {
            return new ResponseDto<string>()
            {
                Message = supervisionCohort.Message, IsSuccess = false, Result = ErrorMessages.DefaultError
            };
        }

        //check if the supervisor has any slots remaining
        if (supervisionCohort.Result != null && supervisionCohort.Result.SupervisionSlot == 0)
        {
            return new ResponseDto<string>()
            {
                Message = "There are no supervision slots available for this supervisor", IsSuccess = false, Result = ErrorMessages.DefaultError
            };
        }

        //check if the supervisor has been disabled
        if (supervisionCohort.Result != null && supervisionCohort.Result.Supervisor.IsLockedOutByAdmin )
        {
            return new ResponseDto<string>()
            {
                Message = "The supervisor is currently disabled by the admin", IsSuccess = false, Result = ErrorMessages.DefaultError
            };
        }
        #endregion


        var supervisionRequest = SupervisionRequest.Create(
            request.SupervisorId,
            studentId,
            dissertationCohort.Result.Id
        );
        await this._unitOfWork.SupervisionRequestRepository.AddAsync(supervisionRequest);
        await this._unitOfWork.SaveAsync(cancellationToken);
        return new ResponseDto<string>()
        {
            Message = "Supervision Request created Successfully",
            Result = SuccessMessages.DefaultSuccess,
            IsSuccess = true
        };
    }
}