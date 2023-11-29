using Dissertation.Application.DTO.Response;
using Dissertation.Application.Utility;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Options;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;
using Shared.MessageBus;
using Shared.Settings;

namespace Dissertation.Application.SupervisorInvite.Commands.UpdateSupervisorInvite;

public class UpdateSupervisorInviteCommandHandler : IRequestHandler<UpdateSupervisorInviteCommand, ResponseDto<GetSupervisorInvite>>
{
    private readonly IAppLogger<UpdateSupervisorInviteCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;
    private readonly ServiceBusSettings _serviceBusSettings;
    private readonly ApplicationUrlSettings _applicationUrlSettings;
    private readonly IMessageBus _messageBus;

    public UpdateSupervisorInviteCommandHandler(IAppLogger<UpdateSupervisorInviteCommandHandler> logger, IUnitOfWork db, IMapper mapper, IOptions<ServiceBusSettings> serviceBusSettings, IOptions<ApplicationUrlSettings> applicationUrlSettings, IMessageBus messageBus)
    {
        this._logger = logger;
        this._db = db;
        this._mapper = mapper;
        this._serviceBusSettings = serviceBusSettings.Value;
        this._applicationUrlSettings = applicationUrlSettings.Value;
        this._messageBus = messageBus;
    }

    public async Task<ResponseDto<GetSupervisorInvite>> Handle(UpdateSupervisorInviteCommand request,
        CancellationToken cancellationToken)
    {
        var response = new ResponseDto<GetSupervisorInvite>();
        //fetch the supervision invite from the database
        Domain.Entities.SupervisorInvite? supervisorInvite = await this._db.SupervisorInviteRepository.GetFirstOrDefaultAsync(a => a.Id == request.Id);
        if (supervisorInvite == null)
        {
            this._logger.LogError("No Supervision Invite found with {ID}", request.Id);
            throw new NotFoundException(nameof(Domain.Entities.Department), request.Id);
        }

        var emailFromDatabase = supervisorInvite.Email;

        //update the database
        supervisorInvite.FirstName = request.FirstName;
        supervisorInvite.Email = request.Email;
        supervisorInvite.StaffId = request.StaffId;
        supervisorInvite.LastName = request.LastName;

        //send another email if the email was modified
        if (request.Email != emailFromDatabase)
        {
            var code = InviteCodeGenerator.GenerateCode(8);
            await PublishSupervisionInviteMessage(request, code);
            supervisorInvite.InvitationCode = code;
            supervisorInvite.ExpiryDate = DateTime.Today.Add(TimeSpan.FromDays(7)).Date;
        }

        this._db.SupervisorInviteRepository.Update(supervisorInvite);
        await this._db.SaveAsync(cancellationToken);

        GetSupervisorInvite mappedSupervisionInvite = this._mapper.Map<GetSupervisorInvite>(supervisorInvite);
        mappedSupervisionInvite.UpdateStatus();

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedSupervisionInvite;
        return response;
    }

    private async Task PublishSupervisionInviteMessage(UpdateSupervisorInviteCommand request, string invitationCode)
    {
        //send an email to the user's email
        var callbackUrl = CallbackUrlGenerator.GenerateSupervisionInviteCallBackUrl(
            this._applicationUrlSettings.WebClientUrl, this._applicationUrlSettings.SupervisorConfirmInviteRoute,
            request.StaffId, invitationCode);

        var userDto = new UserDto
        {
            UserName = request.StaffId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email
        };
        var emailDto = new PublishEmailDto { User = userDto, CallbackUrl = callbackUrl, EmailType = EmailType.EmailTypeResetPasswordEmail };
        await this._messageBus.PublishMessage(emailDto, this._serviceBusSettings.EmailLoggerQueue,
            this._serviceBusSettings.ServiceBusConnectionString);
    }
}