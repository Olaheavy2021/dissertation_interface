using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using FluentValidation;
using Shared.Constants;
using Shared.DTO;

namespace Dissertation.Application.SupervisorInvite.Commands.UpdateSupervisorInvite;

public class UpdateSupervisorInviteCommandValidator : AbstractValidator<UpdateSupervisorInviteCommand>
{
    private readonly IUnitOfWork _db;
    private readonly IUserApiService _userApiService;

    public UpdateSupervisorInviteCommandValidator(IUnitOfWork db, IUserApiService userApiService)
    {
        this._db = db;
        this._userApiService = userApiService;

        RuleFor(p => p.FirstName)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .NotNull().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace);

        RuleFor(p => p.LastName)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .NotNull().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace);

        RuleFor(p => p.StaffId)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .NotNull().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace);

        RuleFor(p => p.Email)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .NotNull().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(100).WithMessage(ErrorMessages.MaximumLength100)
            .Matches(@"^[a-zA-Z0-9._%+-]+@(student\.shu\.ac\.uk|shu\.ac\.uk|hallam\.shu\.ac\.uk)$").WithMessage(ErrorMessages.MustBeHallamEmailFormat)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace)
            .EmailAddress();

        RuleFor(q => q)
            .MustAsync(DoesUserWithEmailExists)
            .WithMessage("This email already exists for a user")
            .OverridePropertyName("Email");

        RuleFor(q => q)
            .MustAsync(DoesUserWithUserNameExists)
            .WithMessage("This username already exists for a user")
            .OverridePropertyName("StaffId");

        RuleFor(q => q)
            .MustAsync(IsSupervisionInviteActive)
            .WithMessage("Supervision Invite is not active")
            .OverridePropertyName("Id");
    }

    private async Task<bool> DoesUserWithUserNameExists(UpdateSupervisorInviteCommand request, CancellationToken token)
    {
        ResponseDto<GetUserDto> response = await this._userApiService.GetUserByUserName(request.StaffId);
        return !response.IsSuccess;
    }

    private async Task<bool> DoesUserWithEmailExists(UpdateSupervisorInviteCommand request, CancellationToken token)
    {
        ResponseDto<GetUserDto> response = await this._userApiService.GetUserByEmail(request.Email);
        return !response.IsSuccess;
    }

    private async Task<bool> IsSupervisionInviteActive(UpdateSupervisorInviteCommand request, CancellationToken token)
    {
        Domain.Entities.SupervisorInvite? supervisionInvite = await this._db.SupervisorInviteRepository.GetFirstOrDefaultAsync(x => x.Id == request.Id);
        return supervisionInvite != null && supervisionInvite.ExpiryDate.Date > DateTime.UtcNow.Date;
    }
}