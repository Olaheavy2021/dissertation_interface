using FluentValidation;
using Shared.Constants;
using UserManagement_API.Data.Models.Dto;

namespace UserManagement_API.Data.Models.Validators;

public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(p => p.Password)
            .NotNull().WithMessage(ErrorMessages.RequiredField)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField);

        RuleFor(p => p.Email)
            .NotNull().WithMessage(ErrorMessages.RequiredField)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .Matches(@"^[a-zA-Z0-9._%+-]+@(student\.shu\.ac\.uk|shu\.ac\.uk|hallam\.shu\.ac\.uk|shu\.com)$")
            .WithMessage("Invalid email format.");
    }
}