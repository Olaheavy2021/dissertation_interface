using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Helpers;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Options;
using Shared.Constants;
using Shared.DTO;
using Shared.Logging;
using Shared.MessageBus;
using Shared.Settings;

namespace Dissertation.Application.StudentInvite.Commands.ResendStudentInvite;

public class ResendStudentInviteCommandHandler : IRequestHandler<ResendStudentInviteCommand, ResponseDto<GetStudentInvite>>
{
    private readonly IAppLogger<ResendStudentInviteCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;
    private readonly IMessageBus _messageBus;
    private readonly ServiceBusSettings _serviceBusSettings;
    private readonly ApplicationUrlSettings _applicationUrlSettings;

    public ResendStudentInviteCommandHandler(IAppLogger<ResendStudentInviteCommandHandler> logger, IUnitOfWork db, IMessageBus messageBus, IMapper mapper, IOptions<ApplicationUrlSettings> applicationUrlSettings, IOptions<ServiceBusSettings> serviceBusSettings)
    {
        this._logger = logger;
        this._db = db;
        this._messageBus = messageBus;
        this._mapper = mapper;
        this._applicationUrlSettings = applicationUrlSettings.Value;
        this._serviceBusSettings = serviceBusSettings.Value;
    }

    public async Task<ResponseDto<GetStudentInvite>> Handle(ResendStudentInviteCommand request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to Resend Student Invite for {inviteId}", request.InviteId);
        var response = new ResponseDto<GetStudentInvite>();
        Domain.Entities.StudentInvite? studentInvite =
            await this._db.StudentInviteRepository.GetFirstOrDefaultAsync(x =>
                x.Id == request.InviteId, includes: x => x.DissertationCohort);

        if (studentInvite == null)
        {
            response.IsSuccess = false;
            response.Message = "Invalid Student Invitation. Please contact admin";
            return response;
        }

        //extend the expiry by 7 days again
        studentInvite.ExpiryDate = DateTime.UtcNow.Date.AddDays(7);
        this._db.StudentInviteRepository.Update(studentInvite);

        //resend the email
        var callbackUrl = CallbackUrlGenerator.GenerateStudentInviteCallBackUrl(
            this._applicationUrlSettings.WebClientUrl, this._applicationUrlSettings.StudentConfirmInviteRoute,
            studentInvite.StudentId, studentInvite.InvitationCode);

        var userDto = new UserDto
        {
            UserName = studentInvite.StudentId,
            FirstName = studentInvite.FirstName,
            LastName = studentInvite.LastName,
            Email = studentInvite.Email
        };

        var emailDto = new PublishEmailDto { User = userDto, CallbackUrl = callbackUrl, EmailType = EmailType.EmailTypeStudentInviteEmail };
        await this._messageBus.PublishMessage(emailDto, this._serviceBusSettings.EmailLoggerQueue,
            this._serviceBusSettings.ServiceBusConnectionString);
        //resend the email

        GetStudentInvite mappedStudentInvite = this._mapper.Map<GetStudentInvite>(studentInvite);
        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedStudentInvite;
        return response;
    }
}