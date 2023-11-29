using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.SupervisorInvite.Queries.GetById;

public class GetSupervisionInviteByIdQueryHandler : IRequestHandler<GetSupervisionInviteByIdQuery,ResponseDto<GetSupervisorInvite>>
{
    private readonly IAppLogger<GetSupervisionInviteByIdQueryHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public GetSupervisionInviteByIdQueryHandler(IAppLogger<GetSupervisionInviteByIdQueryHandler> logger, IUnitOfWork db, IMapper mapper)
    {
        this._logger = logger;
        this._db = db;
        this._mapper = mapper;
    }

    public async Task<ResponseDto<GetSupervisorInvite>> Handle(GetSupervisionInviteByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new ResponseDto<GetSupervisorInvite>();
        this._logger.LogInformation("Attempting to retrieve a Supervision Invite by ID {SupervisionInviteID}", request.Id);
        Domain.Entities.SupervisorInvite? supervisorInvite = await this._db.SupervisorInviteRepository.GetAsync(a => a.Id == request.Id);
        if (supervisorInvite is null)
        {
            this._logger.LogError("No Supervision Invite with ID");
            throw new NotFoundException(nameof(Domain.Entities.SupervisorInvite), request.Id);
        }

        GetSupervisorInvite mappedSupervisorInvite = this._mapper.Map<GetSupervisorInvite>(supervisorInvite);
        mappedSupervisorInvite.UpdateStatus();

        this._logger.LogInformation("Successfully retrieved a Supervisor Invite by ID {SupervisionInviteID}", request.Id);
        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedSupervisorInvite;
        return response;
    }
}