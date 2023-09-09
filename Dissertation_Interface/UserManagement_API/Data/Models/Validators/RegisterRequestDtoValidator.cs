using FluentValidation;
using Shared.Constants;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models.Dto;

namespace UserManagement_API.Data.Models.Validators;

public class RegisterRequestDtoValidator: AbstractValidator<RegistrationRequestDto>
{
    private readonly IUnitOfWork _db;

    public RegisterRequestDtoValidator(IUnitOfWork db)
    {
        this._db = db;
        RuleFor(p => p.FirstName)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .NotNull().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50);

        RuleFor(p => p.LastName)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .NotNull().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50);

        RuleFor(p => p.UserName)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .NotNull().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50);

        RuleFor(p => p.Email)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .NotNull().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(100).WithMessage(ErrorMessages.MaximumLength100)
            .Matches(@"^[a-zA-Z0-9._%+-]+@hallam\.shu\.ac\.uk$").WithMessage(ErrorMessages.MustBeHallamEmailFormat)
            .EmailAddress();

        RuleFor(p => p.Password)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .NotNull().WithMessage(ErrorMessages.RequiredField)
            .MinimumLength(8).WithMessage(ErrorMessages.PasswordMinimumLength)
            .Matches(@"[A-Z]+").WithMessage(ErrorMessages.MustContainUppercase)
            .Matches(@"[a-z]+").WithMessage(ErrorMessages.MustContainLowercase)
            .Matches(@"[0-9]+").WithMessage(ErrorMessages.MustContainNumber)
            .Matches(@".*[!@#$%^&*()_+{}\[\]:;<>,.?~\\-].*").WithMessage(ErrorMessages.MustContainSpecialCharacter);

        RuleFor(q => q)
            .MustAsync(IsUserDetailsUnique).WithMessage(ErrorMessages.AccountAlreadyExists)
            .OverridePropertyName("Custom");
    }

    private async Task<bool> IsUserDetailsUnique(RegistrationRequestDto request, CancellationToken token) => !await this._db.ApplicationUserRepository.AnyAsync(a => a.NormalizedUserName == request.UserName.ToUpper() || a.NormalizedEmail == request.Email.ToUpper());
}