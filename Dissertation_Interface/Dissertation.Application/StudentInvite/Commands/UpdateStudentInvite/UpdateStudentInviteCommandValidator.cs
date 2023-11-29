using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using FluentValidation;
using Shared.Constants;
using Shared.DTO;

namespace Dissertation.Application.StudentInvite.Commands.UpdateStudentInvite;

public class UpdateStudentInviteCommandValidator : AbstractValidator<UpdateStudentInviteCommand>
{
    private readonly IUnitOfWork _db;
    private readonly IUserApiService _userApiService;

    public UpdateStudentInviteCommandValidator(IUnitOfWork db, IUserApiService userApiService)
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

        RuleFor(p => p.StudentId)
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
            .WithMessage("This email already exists for a User")
            .OverridePropertyName("Email");

        RuleFor(q => q)
            .MustAsync(DoesUserWithUserNameExists)
            .WithMessage("This username already exists for a User")
            .OverridePropertyName("StaffId");

        RuleFor(q => q)
            .MustAsync(IsStudentInviteActive)
            .WithMessage("Student Invite is not active")
            .OverridePropertyName("Id");
    }

    private async Task<bool> DoesUserWithEmailExists(UpdateStudentInviteCommand request, CancellationToken token)
    {
        ResponseDto<GetUserDto> response = await this._userApiService.GetUserByEmail(request.Email);
        return !response.IsSuccess;
    }

    private async Task<bool> DoesUserWithUserNameExists(UpdateStudentInviteCommand request, CancellationToken token)
    {
        ResponseDto<GetUserDto> response = await this._userApiService.GetUserByUserName(request.StudentId);
        return !response.IsSuccess;
    }
    private async Task<bool> IsStudentInviteActive(UpdateStudentInviteCommand request, CancellationToken token)
    {
        Domain.Entities.StudentInvite? studentInvite = await this._db.StudentInviteRepository.GetFirstOrDefaultAsync(x => x.Id == request.Id);
        return studentInvite != null && studentInvite.ExpiryDate.Date > DateTime.UtcNow.Date;
    }
}