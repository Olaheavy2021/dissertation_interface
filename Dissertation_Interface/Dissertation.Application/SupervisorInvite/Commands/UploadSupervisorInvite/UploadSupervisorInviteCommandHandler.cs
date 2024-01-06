using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.DTO;
using Dissertation.Infrastructure.Persistence.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using Shared.DTO;
using Shared.MessageBus;

namespace Dissertation.Application.SupervisorInvite.Commands.UploadSupervisorInvite;

public class UploadSupervisorInviteCommandHandler : IRequestHandler<UploadSupervisorInviteCommand, ResponseDto<string>>
{
    private readonly IUnitOfWork _db;
    private readonly IUserApiService _userApiService;
    private readonly IMessageBus _messageBus;

    public UploadSupervisorInviteCommandHandler(IUnitOfWork db, IUserApiService userApiService, IMessageBus messageBus)
    {
        this._db = db;
        this._userApiService = userApiService;
        this._messageBus = messageBus;
    }

    public async Task<ResponseDto<string>> Handle(UploadSupervisorInviteCommand command, CancellationToken cancellationToken)
    {
        var bulkUploadRequest = new BulkUserUploadRequest { Requests = new List<UserUploadRequest>() };
        var invalidSupervisorInvites = new List<string>();
        foreach (UserUploadRequest supervisorInvite in command.Requests)
        {
            var emailExists = await DoesUserWithEmailExists(supervisorInvite.Email);
            var userNameExists = await DoesUserWithUserNameExists(supervisorInvite.Username);
            var supervisorInviteExists = await DoesRequestHaveActiveInvite(supervisorInvite.Email, supervisorInvite.Username);

            var isSupervisorInviteValid = emailExists && userNameExists && supervisorInviteExists;
            if (isSupervisorInviteValid)
            {
                bulkUploadRequest.Requests.Add(supervisorInvite);
            }

            invalidSupervisorInvites.Add($"${supervisorInvite.Email}");
        }

        if (invalidSupervisorInvites.Any())
        {
            return new ResponseDto<string>
            {
                IsSuccess = false,
                Message =
                    "Email or Username already exists for the users, or there is already an active invite for the supervisor",
                Result = string.Join(", ", invalidSupervisorInvites)
            };
        }

        //TODO: Publish the message to the Message Bus
        bulkUploadRequest.BatchUploadType = BatchUploadType.SupervisorInvite;
        return new ResponseDto<string>()
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
        Domain.Entities.SupervisorInvite? supervisorInvite = await this._db.SupervisorInviteRepository
            .GetFirstOrDefaultAsync(x =>
                (EF.Functions.Like(x.StaffId, userName) || EF.Functions.Like(x.Email, email))
                && x.ExpiryDate.Date > DateTime.UtcNow.Date);
        return supervisorInvite == null;
    }
}