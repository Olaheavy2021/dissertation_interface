using Dissertation.Application.DTO.Response;
using Dissertation.Application.SupervisorInvite.Commands.CreateSupervisorInvite;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Logging;

namespace Dissertation.Application.SupervisorInvite.Commands.ConfirmSupervisorInvite;

public class ConfirmSupervisorInviteCommandHandler : IRequestHandler<ConfirmSupervisorInviteCommand, ResponseDto<GetSupervisorInvite>>
{
    private readonly IAppLogger<ConfirmSupervisorInviteCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public ConfirmSupervisorInviteCommandHandler(IAppLogger<ConfirmSupervisorInviteCommandHandler> logger, IUnitOfWork db, IMapper mapper)
    {
        this._logger = logger;
        this._db = db;
        this._mapper = mapper;
    }
    public async Task<ResponseDto<GetSupervisorInvite>> Handle(ConfirmSupervisorInviteCommand request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to Create Supervisor Invite for {username}", request.StaffId);
        var response = new ResponseDto<GetSupervisorInvite>();
        Domain.Entities.SupervisorInvite? supervisorInvite =
            await this._db.SupervisorInviteRepository.GetFirstOrDefaultAsync(x =>
                x.StaffId == request.StaffId && x.InvitationCode == request.InvitationCode);

        if (supervisorInvite == null)
        {
            response.IsSuccess = false;
            response.Message = "Invalid Invitation Code. Please contact admin";
            return response;
        }

        if (DateTime.UtcNow.Date > supervisorInvite.ExpiryDate.Date)
        {
            response.IsSuccess = false;
            response.Message = "Invitation Code has Expired. Please contact admin";
            return response;
        }

        GetSupervisorInvite mappedSupervisionInvite = this._mapper.Map<GetSupervisorInvite>(supervisorInvite);
        mappedSupervisionInvite.UpdateStatus();

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedSupervisionInvite;

        return response;
    }
}