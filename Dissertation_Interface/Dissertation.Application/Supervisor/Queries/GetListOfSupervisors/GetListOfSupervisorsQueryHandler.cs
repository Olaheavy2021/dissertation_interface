using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.DTO;
using Shared.Logging;

namespace Dissertation.Application.Supervisor.Queries.GetListOfSupervisors;

public class GetListOfSupervisorsQueryHandler: IRequestHandler<GetListOfSupervisorsQuery, ResponseDto<PaginatedUserListDto>>
{
    private readonly IAppLogger<GetListOfSupervisorsQueryHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;
    private readonly IUserApiService _userApiService;

    public GetListOfSupervisorsQueryHandler(IAppLogger<GetListOfSupervisorsQueryHandler> logger, IUnitOfWork db, IMapper mapper, IUserApiService userApiService)
    {
        this._logger = logger;
        this._db = db;
        this._mapper = mapper;
        this._userApiService = userApiService;
    }


    public Task<ResponseDto<PaginatedUserListDto>> Handle(GetListOfSupervisorsQuery request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to retrieve list of supervisors");
        Task<ResponseDto<PaginatedUserListDto>> supervisors = this._userApiService.GetListOfSupervisors(request.Parameters);
        return supervisors;

    }
}