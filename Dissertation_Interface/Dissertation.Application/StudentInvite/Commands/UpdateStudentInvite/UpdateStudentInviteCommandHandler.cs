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

namespace Dissertation.Application.StudentInvite.Commands.UpdateStudentInvite;

public class UpdateStudentInviteCommandHandler : IRequestHandler<UpdateStudentInviteCommand, ResponseDto<GetStudentInvite>>
{
    private readonly IAppLogger<UpdateStudentInviteCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;
    private readonly ServiceBusSettings _serviceBusSettings;
    private readonly ApplicationUrlSettings _applicationUrlSettings;
    private readonly IMessageBus _messageBus;

    public UpdateStudentInviteCommandHandler(IAppLogger<UpdateStudentInviteCommandHandler> logger, IUnitOfWork db, IMapper mapper, IOptions<ServiceBusSettings> serviceBusSettings, IOptions<ApplicationUrlSettings> applicationUrlSettings, IMessageBus messageBus)
    {
        this._logger = logger;
        this._db = db;
        this._mapper = mapper;
        this._serviceBusSettings = serviceBusSettings.Value;
        this._applicationUrlSettings = applicationUrlSettings.Value;
        this._messageBus = messageBus;
    }

    public async Task<ResponseDto<GetStudentInvite>> Handle(UpdateStudentInviteCommand request,
        CancellationToken cancellationToken)
    {
        var response = new ResponseDto<GetStudentInvite>();
        //fetch the student invite from the database
        Domain.Entities.StudentInvite? studentInvite = await this._db.StudentInviteRepository.GetFirstOrDefaultAsync(a => a.Id == request.Id, includes: x=> x.DissertationCohort);
        if (studentInvite == null)
        {
            this._logger.LogError("No Student Invite found with {ID}", request.Id);
            throw new NotFoundException(nameof(Domain.Entities.Department), request.Id);
        }

        var emailFromDatabase = studentInvite.Email;

        //update the database
        studentInvite.FirstName = request.FirstName;
        studentInvite.Email = request.Email;
        studentInvite.StudentId = request.StudentId;
        studentInvite.LastName = request.LastName;

        //send another email if the email was modified
        if (request.Email != emailFromDatabase)
        {
            var code = InviteCodeGenerator.GenerateCode(8);
            await PublishStudentInviteMessage(request, code);
            studentInvite.InvitationCode = code;
            studentInvite.ExpiryDate = DateTime.Today.Add(TimeSpan.FromDays(7)).Date;
        }

        this._db.StudentInviteRepository.Update(studentInvite);
        await this._db.SaveAsync(cancellationToken);

        GetStudentInvite mappedStudentInvite = this._mapper.Map<GetStudentInvite>(studentInvite);
        mappedStudentInvite.UpdateStatus();

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedStudentInvite;
        return response;
    }

    private async Task PublishStudentInviteMessage(UpdateStudentInviteCommand request, string invitationCode)
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