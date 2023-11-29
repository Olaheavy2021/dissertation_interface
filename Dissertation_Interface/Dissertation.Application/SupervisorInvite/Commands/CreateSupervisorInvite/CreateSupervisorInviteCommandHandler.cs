using Dissertation.Application.DTO.Response;
using Dissertation.Application.Utility;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Options;
using Shared.Constants;
using Shared.DTO;
using Shared.Helpers;
using Shared.Logging;
using Shared.MessageBus;
using Shared.Settings;

namespace Dissertation.Application.SupervisorInvite.Commands.CreateSupervisorInvite;

public class CreateSupervisorInviteCommandHandler : IRequestHandler<CreateSupervisorInviteCommand, ResponseDto<GetSupervisorInvite>>
{
    private readonly IAppLogger<CreateSupervisorInviteCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;
    private readonly IMessageBus _messageBus;
    private readonly ServiceBusSettings _serviceBusSettings;
    private readonly ApplicationUrlSettings _applicationUrlSettings;

    public CreateSupervisorInviteCommandHandler(IAppLogger<CreateSupervisorInviteCommandHandler> logger, IUnitOfWork db, IMapper mapper, IMessageBus messageBus, IOptions<ServiceBusSettings> serviceBusSettings, IOptions<ApplicationUrlSettings> applicationUrlSettings)
    {
        this._logger = logger;
        this._db = db;
        this._mapper = mapper;
        this._messageBus = messageBus;
        this._serviceBusSettings = serviceBusSettings.Value;
        this._applicationUrlSettings = applicationUrlSettings.Value;
    }

    public async Task<ResponseDto<GetSupervisorInvite>> Handle(CreateSupervisorInviteCommand request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to Create Supervision Invite for this {email}", request.Email);
        var response = new ResponseDto<GetSupervisorInvite>();
        var invitationCode = InviteCodeGenerator.GenerateCode(8);
        var supervisionInvite = Domain.Entities.SupervisorInvite.Create(
           StringHelpers.CapitalizeFirstLetterAndLowercaseRest(request.LastName),
           StringHelpers.CapitalizeFirstLetterAndLowercaseRest(request.FirstName) ,
            request.StaffId.ToLower(),
            request.Email.ToLower(),
            invitationCode
        );

        await this._db.SupervisorInviteRepository.AddAsync(supervisionInvite);
        await this._db.SaveAsync(cancellationToken);
        GetSupervisorInvite mappedSupervisionInvite = this._mapper.Map<GetSupervisorInvite>(supervisionInvite);
        mappedSupervisionInvite.UpdateStatus();

        //publish email
        await PublishSupervisionInviteMessage(request, invitationCode);

        response.Message = "Supervision Invite initiated successfully";
        response.Result = mappedSupervisionInvite;
        response.IsSuccess = true;
        this._logger.LogInformation("Supervision Invite created for this {email}", request.Email);
        return response;
    }

    private async Task PublishSupervisionInviteMessage(CreateSupervisorInviteCommand request, string invitationCode)
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
        var emailDto = new PublishEmailDto { User = userDto, CallbackUrl = callbackUrl, EmailType = EmailType.EmailTypeSupervisorInviteEmail };
        await this._messageBus.PublishMessage(emailDto, this._serviceBusSettings.EmailLoggerQueue,
            this._serviceBusSettings.ServiceBusConnectionString);
    }
}

