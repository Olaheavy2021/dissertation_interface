using Dissertation.Infrastructure.Persistence.IRepository;
using FluentValidation;
using Shared.Constants;

namespace Dissertation.Application.Student.Commands.RegisterStudent;

public class RegisterStudentCommandValidator : AbstractValidator<RegisterStudentCommand>
{
    private readonly IUnitOfWork _db;

    public RegisterStudentCommandValidator(IUnitOfWork db)
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

        RuleFor(p => p.StudentId)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace);

        RuleFor(p => p.InvitationCode)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField);

        RuleFor(p => p.CourseId)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField);

        RuleFor(q => q)
            .MustAsync(DoesCourseExist)
            .WithMessage("This Course does not exist")
            .OverridePropertyName("CourseId");

    }

    private async Task<bool> DoesCourseExist(RegisterStudentCommand request, CancellationToken token)
    {
        Domain.Entities.Course? course = await this._db.CourseRepository.GetAsync(x => x.Id == request.CourseId);
        return course != null;
    }

}