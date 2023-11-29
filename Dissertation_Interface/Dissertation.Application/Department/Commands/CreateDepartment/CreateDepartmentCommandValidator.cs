using Dissertation.Infrastructure.Persistence.IRepository;
using FluentValidation;

namespace Dissertation.Application.Department.Commands.CreateDepartment;

public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    private readonly IUnitOfWork _db;

    public CreateDepartmentCommandValidator(IUnitOfWork db)
    {
        this._db = db;
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Department Name is required");
        RuleFor(x => x)
            .MustAsync(IsDepartmentNameUnique).WithMessage("Department name must be unique")
            .OverridePropertyName("Name");
    }

    private async Task<bool> IsDepartmentNameUnique(CreateDepartmentCommand request, CancellationToken token) => !await this._db.DepartmentRepository.AnyAsync(x => x.Name == request.Name);
}