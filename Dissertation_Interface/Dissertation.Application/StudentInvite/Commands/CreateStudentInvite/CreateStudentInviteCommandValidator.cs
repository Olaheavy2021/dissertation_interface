using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using FluentValidation;
using Shared.Constants;
using Shared.DTO;

namespace Dissertation.Application.StudentInvite.Commands.CreateStudentInvite;

public class CreateStudentInviteCommandValidator : AbstractValidator<CreateStudentInviteCommand>
{
    private readonly IUnitOfWork _db;
    private readonly IUserApiService _userApiService;

    public CreateStudentInviteCommandValidator(IUnitOfWork db, IUserApiService userApiService)
    {
        this._db = db;
        this._userApiService = userApiService;

        RuleFor(p => p.FirstName)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace);

        RuleFor(p => p.LastName)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace);

        RuleFor(p => p.StudentId)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace);

        RuleFor(p => p.Email)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
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
            .OverridePropertyName("StudentId");

        RuleFor(q => q)
            .MustAsync(DoesRequestHaveActiveInvite)
            .WithMessage("The student has an active invite for either the email or the student id")
            .OverridePropertyName("StudentId");
    }

    private async Task<bool> DoesUserWithEmailExists(CreateStudentInviteCommand request, CancellationToken token)
    {
        ResponseDto<GetUserDto> response = await this._userApiService.GetUserByEmail(request.Email);
        return !response.IsSuccess;
    }

    private async Task<bool> DoesUserWithUserNameExists(CreateStudentInviteCommand request, CancellationToken token)
    {
        ResponseDto<GetUserDto> response = await this._userApiService.GetUserByUserName(request.StudentId);
        return !response.IsSuccess;
    }

    private async Task<bool> DoesRequestHaveActiveInvite(CreateStudentInviteCommand request, CancellationToken token)
    {
        Domain.Entities.StudentInvite? studentInvite = await this._db.StudentInviteRepository.GetFirstOrDefaultAsync(x => (
            x.StudentId == request.StudentId.ToLower() || x.Email == request.Email.ToLower()) && x.ExpiryDate.Date > DateTime.UtcNow.Date);

        return studentInvite == null;
    }
}