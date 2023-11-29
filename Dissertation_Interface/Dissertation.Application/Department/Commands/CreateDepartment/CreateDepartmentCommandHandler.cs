using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Enums;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.DTO;
using Shared.Logging;

namespace Dissertation.Application.Department.Commands.CreateDepartment;

public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, ResponseDto<GetDepartment>>
{
    private readonly IAppLogger<CreateDepartmentCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public CreateDepartmentCommandHandler(IAppLogger<CreateDepartmentCommandHandler> logger, IUnitOfWork db, IMapper mapper)
    {
        this._logger = logger;
        this._mapper = mapper;
        this._db = db;
    }

    public async Task<ResponseDto<GetDepartment>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to Create Department for this {name}", request.Name);
        var response = new ResponseDto<GetDepartment>();
        var department = Domain.Entities.Department.Create(request.Name);

        await this._db.DepartmentRepository.AddAsync(department);
        await this._db.SaveAsync(cancellationToken);
        GetDepartment mappedDepartment = this._mapper.Map<GetDepartment>(department);

        response.Message = "Department Created successfully";
        response.Result = mappedDepartment;
        response.IsSuccess = true;
        this._logger.LogInformation("Department created for this {name}", request.Name);
        return response;
    }
}