using Dissertation.Application.AcademicYear.Commands.UpdateAcademicYear;
using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.Department.Commands.UpdateDepartment;

public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, ResponseDto<GetDepartment>>
{
    private readonly IAppLogger<UpdateDepartmentCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public UpdateDepartmentCommandHandler(IAppLogger<UpdateDepartmentCommandHandler> logger, IUnitOfWork db, IMapper mapper)
    {
        this._logger = logger;
        this._db = db;
        this._mapper = mapper;
    }

    public async Task<ResponseDto<GetDepartment>> Handle(UpdateDepartmentCommand request,
        CancellationToken cancellationToken)
    {
        var response = new ResponseDto<GetDepartment>();
        //fetch the academic year from the database
        Domain.Entities.Department? department = await this._db.DepartmentRepository.GetFirstOrDefaultAsync(a => a.Id == request.DepartmentId);
        if (department == null)
        {
            this._logger.LogError("No Department found with {ID}", request.DepartmentId);
            throw new NotFoundException(nameof(Domain.Entities.Department), request.DepartmentId);
        }

        //update the database
        department.Name = request.Name;
        this._db.DepartmentRepository.Update(department);
        await this._db.SaveAsync(cancellationToken);

        GetDepartment mappedDepartment = this._mapper.Map<GetDepartment>(department);
        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedDepartment;
        return response;
    }
}