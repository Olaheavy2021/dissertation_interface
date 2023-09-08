using FluentValidation;
using UserManagement_API.Data.Models.Dto;

namespace UserManagement_API.Data.Models.Validators;

public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(p => p.Password)
            .NotNull().WithMessage("{PropertyName} is required")
            .NotEmpty().WithMessage("{PropertyName} is required");

        RuleFor(p => p.UserName)
            .NotNull().WithMessage("{PropertyName} is required")
            .NotEmpty().WithMessage("{PropertyName} is required");
    }
}