using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.DTO;
using Dissertation.Infrastructure.Persistence.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using Shared.DTO;
using Shared.MessageBus;

namespace Dissertation.Application.StudentInvite.Commands.UploadStudentInvite;

public class UploadStudentInviteCommandHandler : IRequestHandler<UploadStudentInviteCommand, ResponseDto<string>>
{
    private readonly IUnitOfWork _db;
    private readonly IUserApiService _userApiService;
    private readonly IMessageBus _messageBus;

    public UploadStudentInviteCommandHandler(IUnitOfWork db, IUserApiService userApiService, IMessageBus messageBus)
    {
        this._db = db;
        this._userApiService = userApiService;
        this._messageBus = messageBus;
    }

    public async Task<ResponseDto<string>> Handle(UploadStudentInviteCommand command, CancellationToken cancellationToken)
    {
        //Fetch the active dissertation cohort
        Domain.Entities.DissertationCohort? activeCohort = await this._db.DissertationCohortRepository.GetActiveDissertationCohort();
        if (activeCohort == null)
        {
            return new ResponseDto<string>
            {
                IsSuccess = false,
                Message = "There is no active cohort to add students to",
                Result = ErrorMessages.DefaultError
            };
        }

        var bulkUploadRequest = new BulkUserUploadRequest { Requests = new List<UserUploadRequest>() };
        var invalidStudentInvites = new List<string>();
        foreach (UserUploadRequest studentInvite in command.Requests)
        {
            var emailExists = await DoesUserWithEmailExists(studentInvite.Email);
            var userNameExists = await DoesUserWithUserNameExists(studentInvite.Username);
            var studentInviteExists = await DoesRequestHaveActiveInvite(studentInvite.Email, studentInvite.Username);

            var isStudentInviteValid = emailExists && userNameExists && studentInviteExists;
            if (isStudentInviteValid)
            {
                bulkUploadRequest.Requests.Add(studentInvite);
            }

            invalidStudentInvites.Add($"${studentInvite.Email}");
        }

        if (invalidStudentInvites.Any())
        {
            return new ResponseDto<string>
            {
                IsSuccess = false,
                Message =
                    "Email or Username already exists for the users, or there is already an active invite for the student",
                Result = string.Join(", ", invalidStudentInvites)
            };
        }

        bulkUploadRequest.BatchUploadType = BatchUploadType.StudentInvite;
        bulkUploadRequest.ActiveCohortId = activeCohort.Id;
        //TODO: Publish the message to the Message Bus
        return new ResponseDto<string>
        {
            IsSuccess = true,
            Message = SuccessMessages.DefaultSuccess
        };
    }

    private async Task<bool> DoesUserWithEmailExists(string email)
    {
        ResponseDto<GetUserDto> response = await this._userApiService.GetUserByEmail(email);
        return !response.IsSuccess;
    }

    private async Task<bool> DoesUserWithUserNameExists(string userName)
    {
        ResponseDto<GetUserDto> response = await this._userApiService.GetUserByUserName(userName);
        return !response.IsSuccess;
    }

    private async Task<bool> DoesRequestHaveActiveInvite(string email, string userName)
    {
        Domain.Entities.StudentInvite? studentInvite = await this._db.StudentInviteRepository
            .GetFirstOrDefaultAsync(x =>
                (EF.Functions.Like(x.StudentId, userName) || EF.Functions.Like(x.Email, email))
                && x.ExpiryDate.Date > DateTime.UtcNow.Date);
        return studentInvite == null;
    }
}