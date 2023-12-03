using Dissertation.Application.DTO.Response;
using Dissertation.Application.Utility;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Options;
using Shared.Constants;
using Shared.DTO;
using Shared.Logging;
using Shared.MessageBus;
using Shared.Settings;

namespace Dissertation.Application.SupervisorInvite.Commands.ResendSupervisorInvite;

public class ResendSupervisorInviteCommandHandler : IRequestHandler<ResendSupervisorInviteCommand, ResponseDto<GetSupervisorInvite>>
{
    private readonly IAppLogger<ResendSupervisorInviteCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;
    private readonly IMessageBus _messageBus;
    private readonly ServiceBusSettings _serviceBusSettings;
    private readonly ApplicationUrlSettings _applicationUrlSettings;

    public ResendSupervisorInviteCommandHandler(IAppLogger<ResendSupervisorInviteCommandHandler> logger, IUnitOfWork db, IMessageBus messageBus, IMapper mapper, IOptions<ApplicationUrlSettings> applicationUrlSettings,  IOptions<ServiceBusSettings> serviceBusSettings)
    {
        this._logger = logger;
        this._db = db;
        this._messageBus = messageBus;
        this._mapper = mapper;
        this._applicationUrlSettings = applicationUrlSettings.Value;
        this._serviceBusSettings = serviceBusSettings.Value;
    }

    public async Task<ResponseDto<GetSupervisorInvite>> Handle(ResendSupervisorInviteCommand request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to Confirm Supervisor Invite for {username}", request.StaffId);
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

        if (supervisorInvite.ExpiryDate.Date >= DateTime.UtcNow)
        {
            response.IsSuccess = false;
            response.Message = "This invitation is still active. Invalid request";
            return response;
        }

        supervisorInvite.ExpiryDate = DateTime.UtcNow.Date.AddDays(7);
        this._db.SupervisorInviteRepository.Update(supervisorInvite);

        //resend the email
        var callbackUrl = CallbackUrlGenerator.GenerateSupervisionInviteCallBackUrl(
            this._applicationUrlSettings.WebClientUrl, this._applicationUrlSettings.StudentConfirmInviteRoute,
            supervisorInvite.StaffId, supervisorInvite.InvitationCode);

        var userDto = new UserDto
        {
            UserName = supervisorInvite.StaffId,
            FirstName = supervisorInvite.FirstName,
            LastName = supervisorInvite.LastName,
            Email = supervisorInvite.Email
        };

        var emailDto = new PublishEmailDto { User = userDto, CallbackUrl = callbackUrl, EmailType = EmailType.EmailTypeStudentInviteEmail };
        await this._messageBus.PublishMessage(emailDto, this._serviceBusSettings.EmailLoggerQueue,
            this._serviceBusSettings.ServiceBusConnectionString);
        //resend the email

        GetSupervisorInvite mappedSupervisorInvite = this._mapper.Map<GetSupervisorInvite>(supervisorInvite);
        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedSupervisorInvite;
        return response;
    }
}