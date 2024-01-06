using Dissertation.Infrastructure.Persistence.IRepository;
using FluentValidation;
using Shared.Constants;
using Shared.Enums;

namespace Dissertation.Application.Supervisor.Commands.AssignAdminRole;

public class AssignAdminRoleCommandValidator : AbstractValidator<AssignAdminRoleCommand>
{
    private readonly IUnitOfWork _db;

    public AssignAdminRoleCommandValidator(IUnitOfWork db)
    {
        this._db = db;

        RuleFor(p => p.Email)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(100).WithMessage(ErrorMessages.MaximumLength100)
            .Matches(@"^[a-zA-Z0-9._%+-]+@(shu\.ac\.uk|hallam\.shu\.ac\.uk)$").WithMessage(ErrorMessages.MustBeHallamEmailFormat)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace)
            .EmailAddress();

        RuleFor(p => p.Role)
            .IsEnumName(typeof(RolesEnum.AdminRoles), caseSensitive: false).WithMessage(ErrorMessages.MustBeInRoleEnum);
    }
}