using Dissertation.Infrastructure.Persistence.IRepository;
using FluentValidation;
using Shared.Constants;

namespace Dissertation.Application.Supervisor.Commands.AssignSupervisorRole;

public class AssignSupervisorRoleCommandValidator : AbstractValidator<AssignSupervisorRoleCommand>
{
    private readonly IUnitOfWork _db;

    public AssignSupervisorRoleCommandValidator(IUnitOfWork db)
    {
        this._db = db;

        RuleFor(p => p.Email)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(100).WithMessage(ErrorMessages.MaximumLength100)
            .Matches(@"^[a-zA-Z0-9._%+-]+@(shu\.ac\.uk|hallam\.shu\.ac\.uk)$")
            .WithMessage(ErrorMessages.MustBeHallamEmailFormat)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace)
            .EmailAddress();

        RuleFor(p => p.DepartmentId)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField);

        RuleFor(q => q)
            .MustAsync(DoesDepartmentExist)
            .WithMessage("This department does not exist")
            .OverridePropertyName("DepartmentId");
    }

    private async Task<bool> DoesDepartmentExist(AssignSupervisorRoleCommand request, CancellationToken token)
    {
        Domain.Entities.Department? department =
            await this._db.DepartmentRepository.GetAsync(x => x.Id == request.DepartmentId);
        return department != null;
    }
}