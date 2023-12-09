using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Helpers;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;
using UserManagement_API.Helpers;
using UserManagement_API.Service.IService;

namespace UserManagement_API.Service;

public class SupervisionCohortService : ISupervisionCohortService
{
    private readonly IUnitOfWork _db;
    private readonly ILogger<SupervisionCohortService> _logger;
    private readonly IDissertationApiService _dissertationApiService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    public SupervisionCohortService(IUnitOfWork db, ILogger<SupervisionCohortService> logger, UserManager<ApplicationUser> userManager, IMapper mapper, IDissertationApiService dissertationApiService)
    {
        this._db = db;
        this._logger = logger;
        this._userManager = userManager;
        this._mapper = mapper;
        this._dissertationApiService = dissertationApiService;
    }

    public async Task<ResponseDto<string>> CreateSupervisionCohort(CreateSupervisionCohortListRequest request, CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to Add Supervisors to a Supervision Cohort");

        //check if the dissertation cohort is valid
        ResponseDto<GetDissertationCohort> dissertationCohort = await this._dissertationApiService.GetActiveDissertationCohort();
        if (dissertationCohort.Result != null && dissertationCohort.Result.Id != request.DissertationCohortId)
        {
            return new ResponseDto<string>
            {
                Message = $"Invalid Dissertation cohort - {request.DissertationCohortId}",
                Result = ErrorMessages.DefaultError,
                IsSuccess = false
            };
        }

        IList<SupervisionCohort> supervisionCohorts = new List<SupervisionCohort>();
        foreach (CreateSupervisionCohortRequest createSupervisionCohortRequest in request.SupervisionCohortRequests!)
        {
            //check if the user is not null
            ApplicationUser? user = await this._db.ApplicationUserRepository.GetFirstOrDefaultAsync(a => a.Id == createSupervisionCohortRequest.UserId);
            if (user == null)
            {
                this._logger.LogWarning("This supervisor - {userId} has does not exist", createSupervisionCohortRequest.UserId);
                continue;
            }

            //check if the user is a supervisor
            IList<string> roles = await this._userManager.GetRolesAsync(user);
            if (!roles.Any(role => role.Equals(Roles.RoleSupervisor, StringComparison.OrdinalIgnoreCase)))
            {
                this._logger.LogWarning("This user - {userId} is not a supervisor", createSupervisionCohortRequest.UserId);
                continue;
            }

            //check if the supervisor has not been deactivated
            if (user.IsLockedOutByAdmin)
            {
                this._logger.LogWarning("This supervisor - {userId} has been disabled by the admin", createSupervisionCohortRequest.UserId);
                continue;
            }

            //check if the supervisor has not been added before for this cohort
            var doesSupervisionCohortExist = await this._db.SupervisionCohortRepository.AnyAsync(x =>
                x.SupervisorId == createSupervisionCohortRequest.UserId && x.DissertationCohortId == request.DissertationCohortId);

            if (doesSupervisionCohortExist)
            {
                this._logger.LogInformation("This supervisor - {userId} has been added before", createSupervisionCohortRequest.UserId);
                continue;
            }

            var supervisionCohort = SupervisionCohort.Create(createSupervisionCohortRequest.UserId,
                createSupervisionCohortRequest.SupervisionSlot, request.DissertationCohortId);
            supervisionCohorts.Add(supervisionCohort);
        }

        await this._db.SupervisionCohortRepository.AddRangeAsync((IReadOnlyList<SupervisionCohort>)supervisionCohorts);
        await this._db.SaveAsync(cancellationToken);
        var count = supervisionCohorts.Count;
        return new ResponseDto<string>()
        {
            Message = $"{count} Supervisors added to the active cohort successfully",
            Result = SuccessMessages.DefaultSuccess,
            IsSuccess = true
        };
    }

    public async Task<ResponseDto<GetSupervisionCohort>> GetSupervisionCohort(long id)
    {
        this._logger.LogInformation("Attempting to fetch a Supervision Cohort with this {id}", id);
        SupervisionCohort? supervisionCohort =
            await this._db.SupervisionCohortRepository.GetAsync(x => x.Id == id, includes: x => x.Supervisor);

        if (supervisionCohort == null)
        {
            throw new NotFoundException(nameof(SupervisionCohort), id);
        }

        ResponseDto<IReadOnlyList<GetDepartment>> departments = await this._dissertationApiService.GetAllDepartments();
        if (departments == null || departments.Result == null) throw new NotFoundException("Departments", "all");
        this._logger.LogInformation("Number of departments - {count}", departments.Result.Count);


        SupervisorListDto? supervisor = this._mapper.Map<SupervisorListDto>(supervisionCohort.Supervisor);
        supervisor.Department = departments.Result.FirstOrDefault(x => x.Id == supervisionCohort.Supervisor.DepartmentId)!; // Map department

        return new ResponseDto<GetSupervisionCohort>
        {
            IsSuccess = true,
            Message = SuccessMessages.DefaultSuccess,
            Result = new GetSupervisionCohort
            {
                Id = supervisionCohort.Id,
                SupervisionSlot = supervisionCohort.SupervisionSlot,
                DissertationCohortId = supervisionCohort.DissertationCohortId,
                UserDetails = supervisor
            }
        };
    }

    public async Task<ResponseDto<PaginatedSupervisionCohortListDto>> GetSupervisionCohorts(SupervisionCohortListParameters parameters)
    {
        var response = new ResponseDto<PaginatedSupervisionCohortListDto>();
        this._logger.LogInformation("Attempting to retrieve list of Supervision Cohorts");
        PagedList<SupervisionCohort> supervisionCohorts = this._db.SupervisionCohortRepository.GetPaginatedListOfSupervisionCohort(parameters);

        ResponseDto<IReadOnlyList<GetDepartment>> departments = await this._dissertationApiService.GetAllDepartments();
        if (departments == null || departments.Result == null) throw new NotFoundException("Departments", "all");
        this._logger.LogInformation("Number of departments - {count}", departments.Result.Count);


        var mappedDissertationCohort = new PagedList<GetSupervisionCohort>(
            supervisionCohorts.Select(supervisionCohort => MapToSupervisionCohortDto(supervisionCohort, departments.Result) ).ToList(),
            supervisionCohorts.TotalCount,
            supervisionCohorts.CurrentPage,
            supervisionCohorts.PageSize
        );

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = new PaginatedSupervisionCohortListDto
        {
            Data = mappedDissertationCohort,
            TotalCount = mappedDissertationCohort.TotalCount,
            PageSize = mappedDissertationCohort.PageSize,
            CurrentPage = mappedDissertationCohort.CurrentPage,
            TotalPages = mappedDissertationCohort.TotalPages,
            HasNext = mappedDissertationCohort.HasNext,
            HasPrevious = mappedDissertationCohort.HasPrevious
        };

        return response;
    }

    public ResponseDto<PaginatedUserListDto> GetActiveSupervisorsForCohort(
        SupervisionCohortListParameters paginationParameters)
    {
        var response = new ResponseDto<PaginatedUserListDto>();
        PagedList<ApplicationUser> users = this._db.SupervisionCohortRepository.GetActiveSupervisors(paginationParameters);

        var userDtos = new PagedList<UserListDto>(
            users.Select(CustomMappers.MapToUserDto).ToList(),
            users.TotalCount,
            users.CurrentPage,
            users.PageSize
        );

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = new PaginatedUserListDto
        {
            Data = userDtos,
            TotalCount = userDtos.TotalCount,
            PageSize = userDtos.PageSize,
            CurrentPage = userDtos.CurrentPage,
            TotalPages = userDtos.TotalPages,
            HasNext = userDtos.HasNext,
            HasPrevious = userDtos.HasPrevious
        };

        return response;
    }

    public ResponseDto<PaginatedUserListDto> GetInActiveSupervisorsForCohort(
        SupervisionCohortListParameters paginationParameters)
    {
        var response = new ResponseDto<PaginatedUserListDto>();
        PagedList<ApplicationUser> users = this._db.SupervisionCohortRepository.GetInActiveSupervisors(paginationParameters);

        var userDtos = new PagedList<UserListDto>(
            users.Select(CustomMappers.MapToUserDto).ToList(),
            users.TotalCount,
            users.CurrentPage,
            users.PageSize
        );

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = new PaginatedUserListDto
        {
            Data = userDtos,
            TotalCount = userDtos.TotalCount,
            PageSize = userDtos.PageSize,
            CurrentPage = userDtos.CurrentPage,
            TotalPages = userDtos.TotalPages,
            HasNext = userDtos.HasNext,
            HasPrevious = userDtos.HasPrevious
        };

        return response;
    }

    public async Task<ResponseDto<SupervisionCohort>> GetSupervisionCohort(SupervisionCohortParameters parameters)
    {
        SupervisionCohort? supervisionCohort = await this._db.SupervisionCohortRepository.GetFirstOrDefaultAsync(x =>
            x.SupervisorId == parameters.SupervisorId && x.DissertationCohortId == parameters.DissertationCohortId, includes: x => x.Supervisor);
        if (supervisionCohort == null)
        {
            return new ResponseDto<SupervisionCohort>
            {
                IsSuccess = false, Message = "Supervisor has not been assigned to this cohort"
            };
        }

        return new ResponseDto<SupervisionCohort>()
        {
            IsSuccess = true, Message = SuccessMessages.DefaultSuccess, Result = supervisionCohort
        };
    }

    private GetSupervisionCohort MapToSupervisionCohortDto(
        SupervisionCohort supervisionCohort, IEnumerable<GetDepartment> departments)
    {
         SupervisorListDto? mappedUser = this._mapper.Map<SupervisorListDto>(supervisionCohort.Supervisor);
         mappedUser.Department = departments.FirstOrDefault(x => x.Id == supervisionCohort.Supervisor.DepartmentId)!; // Map department
        return new GetSupervisionCohort
        {
            Id = supervisionCohort.Id,
            DissertationCohortId = supervisionCohort.DissertationCohortId,
            UserDetails = mappedUser,
            SupervisionSlot = supervisionCohort.SupervisionSlot
        };
    }
}