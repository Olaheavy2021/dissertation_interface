using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Enums;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.Department.Commands.EnableDepartment;

public class EnableDepartmentCommandHandler : IRequestHandler<EnableDepartmentCommand, ResponseDto<GetDepartment>>
{
    private readonly IAppLogger<EnableDepartmentCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public EnableDepartmentCommandHandler(IAppLogger<EnableDepartmentCommandHandler> logger, IUnitOfWork db,
        IMapper mapper)
    {
        this._logger = logger;
        this._mapper = mapper;
        this._db = db;
    }

    public async Task<ResponseDto<GetDepartment>> Handle(EnableDepartmentCommand request,
        CancellationToken cancellationToken)
    {
        var response = new ResponseDto<GetDepartment>();

        //fetch the department from the database
        Domain.Entities.Department? department =
            await this._db.DepartmentRepository.GetFirstOrDefaultAsync(a => a.Id == request.DepartmentId);

        if (department == null)
        {
            this._logger.LogError("No Department found with {ID}", request.DepartmentId);
            throw new NotFoundException(nameof(Domain.Entities.Department), request.DepartmentId);
        }

        if (department.Status.Equals(DissertationConfigStatus.Active))
        {
            response.IsSuccess = false;
            response.Message = "Department is enabled already. Invalid Request";

            return response;
        }

        department.Status = DissertationConfigStatus.Active;
        this._db.DepartmentRepository.Update(department);
        await this._db.SaveAsync(cancellationToken);

        GetDepartment mappedDepartment = this._mapper.Map<GetDepartment>(department);

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedDepartment;

        return response;
    }
}