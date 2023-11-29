using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Enums;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Logging;

namespace Dissertation.Application.Department.Queries.GetListOfActiveDepartment;

public class GetListOfActiveDepartmentQueryHandler : IRequestHandler<GetListOfActiveDepartmentQuery, ResponseDto<IEnumerable<GetDepartment>>>
{
    private readonly IAppLogger<GetListOfActiveDepartmentQueryHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public GetListOfActiveDepartmentQueryHandler(IAppLogger<GetListOfActiveDepartmentQueryHandler> logger, IUnitOfWork db, IMapper mapper)
    {
        this._db = db;
        this._logger = logger;
        this._mapper = mapper;
    }

    public async Task<ResponseDto<IEnumerable<GetDepartment>>> Handle(GetListOfActiveDepartmentQuery request,
        CancellationToken cancellationToken)
    {
        var response = new ResponseDto<IEnumerable<GetDepartment>>();
        this._logger.LogInformation("Attempting to retrieve list of active Department");

        IReadOnlyList<Domain.Entities.Department> departments = await this._db.DepartmentRepository.GetAllAsync(x => x.Status == DissertationConfigStatus.Active);
        IEnumerable<GetDepartment> mappedDepartment = this._mapper.Map<IEnumerable<GetDepartment>>(departments);

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedDepartment;

        return response;
    }
}