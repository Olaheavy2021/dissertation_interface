using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.SupervisorInvite.Commands.DeleteSupervisorInvite;

public class DeleteSupervisorInviteCommandHandler : IRequestHandler<DeleteSupervisorInviteCommand, ResponseDto<GetSupervisorInvite>>
{
    private readonly IAppLogger<DeleteSupervisorInviteCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public DeleteSupervisorInviteCommandHandler(IAppLogger<DeleteSupervisorInviteCommandHandler> logger, IUnitOfWork db,
        IMapper mapper)
    {
        this._logger = logger;
        this._mapper = mapper;
        this._db = db;
    }

    public async Task<ResponseDto<GetSupervisorInvite>> Handle(DeleteSupervisorInviteCommand request,
        CancellationToken cancellationToken)
    {
        var response = new ResponseDto<GetSupervisorInvite>();

        //fetch the supervision invite from the database
        Domain.Entities.SupervisorInvite? supervisorInvite =
            await this._db.SupervisorInviteRepository.GetFirstOrDefaultAsync(a => a.Id == request.Id);

        if (supervisorInvite == null)
        {
            this._logger.LogError("No Supervision Invite found with {ID}", request.Id);
            throw new NotFoundException(nameof(Domain.Entities.SupervisorInvite), request.Id);
        }

        this._db.SupervisorInviteRepository.Remove(supervisorInvite);
        await this._db.SaveAsync(cancellationToken);

        GetSupervisorInvite mappedSupervisionInvite = this._mapper.Map<GetSupervisorInvite>(supervisorInvite);
        mappedSupervisionInvite.UpdateStatus();

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedSupervisionInvite;

        return response;
    }
}