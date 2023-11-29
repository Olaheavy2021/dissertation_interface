using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.Department.Queries.GetById;

public class GetDepartmentByIdQueryHandler : IRequestHandler<GetDepartmentByIdQuery, ResponseDto<GetDepartment>>
{
    private readonly IAppLogger<GetDepartmentByIdQueryHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public GetDepartmentByIdQueryHandler(IAppLogger<GetDepartmentByIdQueryHandler> logger, IUnitOfWork db, IMapper mapper)
    {
        this._logger = logger;
        this._db = db;
        this._mapper = mapper;
    }

    public async Task<ResponseDto<GetDepartment>> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new ResponseDto<GetDepartment>();
        this._logger.LogInformation("Attempting to retrieve an Department by ID {DepartmentID}", request.DepartmentId);
        Domain.Entities.Department? department = await this._db.DepartmentRepository.GetAsync(a => a.Id == request.DepartmentId);
        if (department is null)
        {
            this._logger.LogError("No Department found with ID");
            throw new NotFoundException(nameof(Domain.Entities.Department), request.DepartmentId);
        }

        GetDepartment mappedDepartment = this._mapper.Map<GetDepartment>(department);

        this._logger.LogInformation("Successfully retrieved an Department by ID {DepartmentID}", department.Id);
        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedDepartment;
        return response;
    }
}