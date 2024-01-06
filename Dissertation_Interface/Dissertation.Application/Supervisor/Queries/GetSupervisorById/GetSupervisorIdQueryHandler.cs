using System.Linq.Expressions;
using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.Supervisor.Queries.GetSupervisorById;

public class GetSupervisorByIdQueryHandler : IRequestHandler<GetSupervisorByIdQuery, ResponseDto<GetSupervisor>>
{
    private readonly IAppLogger<GetSupervisorByIdQueryHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;
    private readonly IUserApiService _userApiService;

    public GetSupervisorByIdQueryHandler(IAppLogger<GetSupervisorByIdQueryHandler> logger, IUnitOfWork db, IMapper mapper, IUserApiService userApiService)
    {
        this._logger = logger;
        this._db = db;
        this._mapper = mapper;
        this._userApiService = userApiService;
    }
    public async Task<ResponseDto<GetSupervisor>> Handle(GetSupervisorByIdQuery request, CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to retrieve a supervisor by Id - {userId}", request.Id);

        // Fetch user details
        ResponseDto<GetUserDto> userResponse = await this._userApiService.GetUserByUserId(request.Id);
        if (!userResponse.IsSuccess || userResponse.Result == null)
        {
            return new ResponseDto<GetSupervisor>
            {
                IsSuccess = false,
                Message = userResponse.Message ?? "User not found."
            };
        }

        // Fetch supervisor details
        Domain.Entities.Supervisor? supervisor = await this._db.SupervisorRepository.GetFirstOrDefaultAsync(
            x => x.UserId == request.Id,
            includes: new Expression<Func<Domain.Entities.Supervisor, object>>[]
            {
                u => u.Department
            }) ?? throw new NotFoundException(nameof(Domain.Entities.Supervisor), request.Id);

        // Map supervisor details and prepare the response
        SupervisorDto mappedSupervisor = this._mapper.Map<SupervisorDto>(supervisor);
        return new ResponseDto<GetSupervisor>
        {
            IsSuccess = true,
            Message = SuccessMessages.DefaultSuccess,
            Result = new GetSupervisor
            {
                UserDetails = userResponse.Result,
                SupervisorDetails = mappedSupervisor
            }
        };
    }
}