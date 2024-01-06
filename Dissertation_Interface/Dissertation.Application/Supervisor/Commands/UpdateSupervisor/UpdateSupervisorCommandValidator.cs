using Dissertation.Infrastructure.Persistence.IRepository;
using FluentValidation;
using Shared.Constants;

namespace Dissertation.Application.Supervisor.Commands.UpdateSupervisor;

public class UpdateSupervisorCommandValidator : AbstractValidator<UpdateSupervisorCommand>
{
    private readonly IUnitOfWork _db;
    public UpdateSupervisorCommandValidator(IUnitOfWork db)
    {
        this._db = db;
        RuleFor(p => p.FirstName)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .NotNull().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace);

        RuleFor(p => p.LastName)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .NotNull().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace);

        RuleFor(p => p.StaffId)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .NotNull().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace);

        RuleFor(p => p.DepartmentId)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField);

        RuleFor(q => q)
            .MustAsync(DoesDepartmentExist)
            .WithMessage("This department does not exist")
            .OverridePropertyName("DepartmentId");

        RuleFor(p => p.Id)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField);
    }

    private async Task<bool> DoesDepartmentExist(UpdateSupervisorCommand request, CancellationToken token)
    {
        Domain.Entities.Department? department = await this._db.DepartmentRepository.GetAsync(x => x.Id == request.DepartmentId);
        return department != null;
    }
}