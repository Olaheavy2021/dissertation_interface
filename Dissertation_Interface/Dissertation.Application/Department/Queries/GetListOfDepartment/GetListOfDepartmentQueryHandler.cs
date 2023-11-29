using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Helpers;
using Shared.Logging;

namespace Dissertation.Application.Department.Queries.GetListOfDepartment;

public class GetListOfDepartmentQueryHandler : IRequestHandler<GetListOfDepartmentQuery, ResponseDto<PaginatedDepartmentListDto>>
{
    private readonly IAppLogger<GetListOfDepartmentQueryHandler> _logger;
    private readonly IUnitOfWork _db;

    public GetListOfDepartmentQueryHandler(IAppLogger<GetListOfDepartmentQueryHandler> logger, IUnitOfWork db)
    {
        this._db = db;
        this._logger = logger;
    }

    public Task<ResponseDto<PaginatedDepartmentListDto>> Handle(GetListOfDepartmentQuery request,
        CancellationToken cancellationToken)
    {
        var response = new ResponseDto<PaginatedDepartmentListDto>();
        this._logger.LogInformation("Attempting to retrieve list of Department");
        PagedList<Domain.Entities.Department> departments = this._db.DepartmentRepository.GetListOfDepartments(request.Parameters);

        var mappedDepartments = new PagedList<GetDepartment>(
            departments.Select(MapToDepartmentDto).ToList(),
            departments.TotalCount,
            departments.CurrentPage,
            departments.PageSize
        );

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = new PaginatedDepartmentListDto()
        {
            Data = mappedDepartments,
            TotalCount = departments.TotalCount,
            PageSize = departments.PageSize,
            CurrentPage = departments.CurrentPage,
            TotalPages = departments.TotalPages,
            HasNext = departments.HasNext,
            HasPrevious = departments.HasPrevious
        };

        return Task.FromResult(response);
    }

    private static GetDepartment MapToDepartmentDto(Domain.Entities.Department department) =>
        new()
        {
            Id = department.Id,
            Status = department.Status,
            UpdatedBy = department.UpdatedBy,
            UpdatedAt = department.UpdatedAt,
            CreatedBy = department.CreatedBy,
            CreatedAt = department.CreatedAt,
            Name = department.Name
        };
}