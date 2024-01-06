using Dissertation.Application.Course.Queries.GetAllCourses;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Logging;

namespace Dissertation.Application.Department.Queries.GetAllDepartments;

public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, ResponseDto<IReadOnlyList<GetDepartment>>>
{
    private readonly IAppLogger<GetAllDepartmentsQueryHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public GetAllDepartmentsQueryHandler(IAppLogger<GetAllDepartmentsQueryHandler> logger, IUnitOfWork db, IMapper mapper)
    {
        this._db = db;
        this._logger = logger;
        this._mapper = mapper;
    }

    public async Task<ResponseDto<IReadOnlyList<GetDepartment>>> Handle(GetAllDepartmentsQuery request,
        CancellationToken cancellationToken)
    {
        var response = new ResponseDto<IReadOnlyList<GetDepartment>>();
        this._logger.LogInformation("Attempting to retrieve list of all Departments");
        IReadOnlyList<Domain.Entities.Department> departments = await this._db.DepartmentRepository.GetAllAsync();

        IReadOnlyList<GetDepartment> mappedDepartment = this._mapper.Map<IReadOnlyList<GetDepartment>>(departments);

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedDepartment;

        return response;
    }
}