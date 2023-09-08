using FluentValidation;
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
            .NotEmpty().WithMessage("{PropertyName} is required")
            .NotNull().WithMessage("{PropertyName} is required")
            .MaximumLength(50).WithMessage("{PropertyName} must be fewer than 50 characters");

        RuleFor(p => p.LastName)
            .NotEmpty().WithMessage("{PropertyName} is required")
            .NotNull().WithMessage("{PropertyName} is required")
            .MaximumLength(50).WithMessage("{PropertyName} must be fewer than 50 characters");

        RuleFor(p => p.UserName)
            .NotEmpty().WithMessage("{PropertyName} is required")
            .NotNull().WithMessage("{PropertyName} is required")
            .MaximumLength(50).WithMessage("{PropertyName} must be fewer than 50 characters");

        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("{PropertyName} is required")
            .NotNull().WithMessage("{PropertyName} is required")
            .MaximumLength(100).WithMessage("{PropertyName} must be fewer than 100 characters")
            .EmailAddress();

        RuleFor(q => q)
            .MustAsync(IsEmailAddressUnique).WithMessage("User with this {PropertyName} already exists ")
            .MustAsync(IsUserNameUnique).WithMessage("User with this {PropertyName} already exists ");
    }

    private async Task<bool> IsEmailAddressUnique(RegistrationRequestDto request, CancellationToken token) => await this._db.ApplicationUserRepository.AnyAsync(a => a.Email == request.Email);

    private async Task<bool> IsUserNameUnique(RegistrationRequestDto request, CancellationToken token) => await this._db.ApplicationUserRepository.AnyAsync(a => a.UserName == request.UserName);
}