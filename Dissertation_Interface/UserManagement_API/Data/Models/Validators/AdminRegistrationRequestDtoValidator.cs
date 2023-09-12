using FluentValidation;
using Shared.Constants;
using Shared.Enums;
using UserManagement_API.Data.Models.Dto;

namespace UserManagement_API.Data.Models.Validators;

public class AdminRegistrationRequestDtoValidator: AbstractValidator<AdminRegistrationRequestDto>
{
    public AdminRegistrationRequestDtoValidator() =>
        RuleFor(p => p.Role)
            .IsEnumName(typeof(RolesEnum.AdminRoles), caseSensitive: false).WithMessage(ErrorMessages.MustBeInRoleEnum);
}