using Dissertation.Application.DTO.Response;
using Dissertation.Application.Utility;
using Dissertation.Infrastructure.Helpers;
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

namespace Dissertation.Application.StudentInvite.Commands.CreateStudentInvite;

public class CreateStudentInviteCommandHandler : IRequestHandler<CreateStudentInviteCommand, ResponseDto<GetStudentInvite>>
{
    private readonly IAppLogger<CreateStudentInviteCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;
    private readonly IMessageBus _messageBus;
    private readonly ServiceBusSettings _serviceBusSettings;
    private readonly ApplicationUrlSettings _applicationUrlSettings;

    public CreateStudentInviteCommandHandler(IAppLogger<CreateStudentInviteCommandHandler> logger, IUnitOfWork db, IMapper mapper, IMessageBus messageBus, IOptions<ServiceBusSettings> serviceBusSettings, IOptions<ApplicationUrlSettings> applicationUrlSettings)
    {
        this._logger = logger;
        this._db = db;
        this._mapper = mapper;
        this._messageBus = messageBus;
        this._serviceBusSettings = serviceBusSettings.Value;
        this._applicationUrlSettings = applicationUrlSettings.Value;
    }

    public async Task<ResponseDto<GetStudentInvite>> Handle(CreateStudentInviteCommand request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to Create Student Invite for this {email}", request.Email);
        var response = new ResponseDto<GetStudentInvite>();

        //get active cohort
        Domain.Entities.DissertationCohort? cohort = await this._db.DissertationCohortRepository.GetActiveDissertationCohort();
        if (cohort == null)
        {
            response.IsSuccess = false;
            response.Message = "Kindly initiate a new and active dissertation cohort before inviting Students";
            return response;
        }

        var invitationCode = InviteCodeGenerator.GenerateCode(8);
        var studentInvite = Domain.Entities.StudentInvite.Create(
           StringHelpers.CapitalizeFirstLetterAndLowercaseRest(request.LastName),
           StringHelpers.CapitalizeFirstLetterAndLowercaseRest(request.FirstName),
            request.StudentId.ToLower(),
            request.Email.ToLower(),
            invitationCode,
           cohort.Id
        );

        await this._db.StudentInviteRepository.AddAsync(studentInvite);
        await this._db.SaveAsync(cancellationToken);
        GetStudentInvite mappedStudentInvite = this._mapper.Map<GetStudentInvite>(studentInvite);
        mappedStudentInvite.UpdateStatus();

        //publish email
        await PublishStudentInviteMessage(request, invitationCode);

        response.Message = "Student Invite initiated successfully";
        response.Result = mappedStudentInvite;
        response.IsSuccess = true;
        this._logger.LogInformation("Student Invite created for this {email}", request.Email);
        return response;
    }

    private async Task PublishStudentInviteMessage(CreateStudentInviteCommand request, string invitationCode)
    {
        //send an email to the user's email
        var callbackUrl = CallbackUrlGenerator.GenerateStudentInviteCallBackUrl(
            this._applicationUrlSettings.WebClientUrl, this._applicationUrlSettings.StudentConfirmInviteRoute,
            request.StudentId, invitationCode);

        var userDto = new UserDto
        {
            UserName = request.StudentId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email
        };

        var emailDto = new PublishEmailDto { User = userDto, CallbackUrl = callbackUrl, EmailType = EmailType.EmailTypeStudentInviteEmail };
        await this._messageBus.PublishMessage(emailDto, this._serviceBusSettings.EmailLoggerQueue,
            this._serviceBusSettings.ServiceBusConnectionString);
    }
}