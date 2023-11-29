using Dissertation.Infrastructure.Persistence.IRepository;
using FluentValidation;
using Shared.Constants;

namespace Dissertation.Application.Supervisor.Commands.RegisterSupervisor;

public class RegisterSupervisorCommandValidator : AbstractValidator<RegisterSupervisorCommand>
{
    private readonly IUnitOfWork _db;

    public RegisterSupervisorCommandValidator(IUnitOfWork db)
    {
        this._db = db;

        RuleFor(p => p.Password)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .MinimumLength(8).WithMessage(ErrorMessages.PasswordMinimumLength)
            .Matches(@"[A-Z]+").WithMessage(ErrorMessages.MustContainUppercase)
            .Matches(@"[a-z]+").WithMessage(ErrorMessages.MustContainLowercase)
            .Matches(@"[0-9]+").WithMessage(ErrorMessages.MustContainNumber)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace)
            .Matches(@".*[!@#$%^&*()_+{}\[\]:;<>,.?~\\-].*").WithMessage(ErrorMessages.MustContainSpecialCharacter);

        RuleFor(p => p.FirstName)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace);

        RuleFor(p => p.LastName)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace);

        RuleFor(p => p.StaffId)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace);

        RuleFor(p => p.InvitationCode)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField);

        RuleFor(p => p.DepartmentId)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField);

        RuleFor(q => q)
            .MustAsync(DoesDepartmentExist)
            .WithMessage("This department does not exist")
            .OverridePropertyName("DepartmentId");

    }

    private async Task<bool> DoesDepartmentExist(RegisterSupervisorCommand request, CancellationToken token)
    {
        Domain.Entities.Department? department = await this._db.DepartmentRepository.GetAsync(x => x.Id == request.DepartmentId);
        return department != null;
    }

}