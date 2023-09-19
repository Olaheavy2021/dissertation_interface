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

        RuleFor(p => p.UserName)
            .NotNull().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(15)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField);
    }
}