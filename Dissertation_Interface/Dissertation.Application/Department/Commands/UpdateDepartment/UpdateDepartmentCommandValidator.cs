using Dissertation.Application.Department.Commands.CreateDepartment;
using Dissertation.Infrastructure.Persistence.IRepository;
using FluentValidation;

namespace Dissertation.Application.Department.Commands.UpdateDepartment;

public class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
{
    private readonly IUnitOfWork _db;

    public UpdateDepartmentCommandValidator(IUnitOfWork db)
    {
        this._db = db;
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Department Name is required");
        RuleFor(x => x)
            .MustAsync(IsDepartmentNameUnique).WithMessage("Department name must be unique or different from the old name")
            .OverridePropertyName("Name");
    }

    private async Task<bool> IsDepartmentNameUnique(UpdateDepartmentCommand request, CancellationToken token) => !await this._db.DepartmentRepository.AnyAsync(x => x.Name == request.Name);
}