using Dissertation.Application.Department.Commands.EnableDepartment;
using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Enums;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.Department.Commands.DisableDepartment;

public class DisableDepartmentCommandHandler : IRequestHandler<DisableDepartmentCommand, ResponseDto<GetDepartment>>
{
    private readonly IAppLogger<DisableDepartmentCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public DisableDepartmentCommandHandler(IAppLogger<DisableDepartmentCommandHandler> logger, IUnitOfWork db,
        IMapper mapper)
    {
        this._logger = logger;
        this._mapper = mapper;
        this._db = db;
    }

    public async Task<ResponseDto<GetDepartment>> Handle(DisableDepartmentCommand request,
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

        if (department.Status.Equals(DissertationConfigStatus.InActive))
        {
            response.IsSuccess = false;
            response.Message = "Department is disabled already. Invalid Request";

            return response;
        }

        department.Status = DissertationConfigStatus.InActive;
        this._db.DepartmentRepository.Update(department);
        await this._db.SaveAsync(cancellationToken);

        GetDepartment mappedDepartment = this._mapper.Map<GetDepartment>(department);

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedDepartment;

        return response;
    }
}