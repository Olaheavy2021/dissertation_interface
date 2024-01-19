using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Helpers;
using Shared.Logging;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;
using UserManagement_API.Helpers;
using UserManagement_API.Service.IService;

namespace UserManagement_API.Service;

public class SupervisionListService : ISupervisionListService
{
    private readonly IAppLogger<SupervisionListService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDissertationApiService _dissertationApiService;

    public SupervisionListService(IUnitOfWork unitOfWork, IAppLogger<SupervisionListService> logger, IDissertationApiService dissertationApiService)
    {
        this._unitOfWork = unitOfWork;
        this._logger = logger;
        this._dissertationApiService = dissertationApiService;
    }

    public async Task<ResponseDto<PaginatedSupervisionListDto>> GetPaginatedListOfSupervisionRequest(
        SupervisionListPaginationParameters parameters)
    {
        this._logger.LogInformation("Fetching a List of SupervisionLists");
        var response = new ResponseDto<PaginatedSupervisionListDto>();
        PagedList<SupervisionList> supervisionLists =
            this._unitOfWork.SupervisionListRepository.GetPaginatedListOfSupervisionLists(parameters);

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

        var data = new PagedList<SupervisionListDto>(
            supervisionLists.Select(supervisionList =>
                    CustomMappers.MapToSupervisionListDto(supervisionList, departments.Result,
                        courses.Result))
                .ToList(),
            supervisionLists.TotalCount,
            supervisionLists.CurrentPage,
            supervisionLists.PageSize
        );

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = new PaginatedSupervisionListDto
        {
            Data = data,
            CurrentPage = supervisionLists.CurrentPage,
            TotalPages = supervisionLists.TotalPages,
            HasNext = supervisionLists.HasNext,
            HasPrevious = supervisionLists.HasPrevious,
            TotalCount = supervisionLists.TotalCount,
            PageSize = supervisionLists.PageSize
        };

        return response;
    }

    public async Task<ResponseDto<PaginatedSupervisionListDto>> GetPaginatedListOfSupervisionListForAStudent(
        SupervisionListPaginationParameters parameters)
    {
        this._logger.LogInformation("Fetching a List of SupervisionList for a Student");
        var response = new ResponseDto<PaginatedSupervisionListDto>();
        PagedList<SupervisionList> supervisionLists =
            this._unitOfWork.SupervisionListRepository.GetSupervisionListsForStudent(parameters);

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

        var data = new PagedList<SupervisionListDto>(
            supervisionLists.Select(supervisionList =>
                    CustomMappers.MapToSupervisionListDto(supervisionList, departments.Result,
                        courses.Result))
                .ToList(),
            supervisionLists.TotalCount,
            supervisionLists.CurrentPage,
            supervisionLists.PageSize
        );

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = new PaginatedSupervisionListDto
        {
            Data = data,
            CurrentPage = supervisionLists.CurrentPage,
            TotalPages = supervisionLists.TotalPages,
            HasNext = supervisionLists.HasNext,
            HasPrevious = supervisionLists.HasPrevious,
            TotalCount = supervisionLists.TotalCount,
            PageSize = supervisionLists.PageSize
        };

        return response;
    }

    public async Task<ResponseDto<PaginatedSupervisionListDto>> GetPaginatedListOfSupervisionListForASupervisor(
        SupervisionListPaginationParameters parameters)
    {
        this._logger.LogInformation("Fetching a List of SupervisionList for a Supervisor");
        var response = new ResponseDto<PaginatedSupervisionListDto>();
        PagedList<SupervisionList> supervisionLists =
            this._unitOfWork.SupervisionListRepository.GetSupervisionListsForSupervisor(parameters);

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

        var data = new PagedList<SupervisionListDto>(
            supervisionLists.Select(supervisionList =>
                    CustomMappers.MapToSupervisionListDto(supervisionList, departments.Result,
                        courses.Result))
                .ToList(),
            supervisionLists.TotalCount,
            supervisionLists.CurrentPage,
            supervisionLists.PageSize
        );

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = new PaginatedSupervisionListDto
        {
            Data = data,
            CurrentPage = supervisionLists.CurrentPage,
            TotalPages = supervisionLists.TotalPages,
            HasNext = supervisionLists.HasNext,
            HasPrevious = supervisionLists.HasPrevious,
            TotalCount = supervisionLists.TotalCount,
            PageSize = supervisionLists.PageSize
        };

        return response;
    }
}