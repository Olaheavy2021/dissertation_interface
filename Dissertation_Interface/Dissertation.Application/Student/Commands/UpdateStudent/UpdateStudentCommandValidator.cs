using Dissertation.Infrastructure.Persistence.IRepository;
using FluentValidation;
using Shared.Constants;

namespace Dissertation.Application.Student.Commands.UpdateStudent;

public class UpdateStudentCommandValidator : AbstractValidator<UpdateStudentCommand>
{
    private readonly IUnitOfWork _db;

    public UpdateStudentCommandValidator(IUnitOfWork db)
    {
        this._db = db;

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

        RuleFor(p => p.CourseId)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField);

        RuleFor(q => q)
            .MustAsync(DoesCourseExist)
            .WithMessage("This Course does not exist")
            .OverridePropertyName("CourseId");

        RuleFor(p => p.Id)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField);
    }

    private async Task<bool> DoesCourseExist(UpdateStudentCommand request, CancellationToken token)
    {
        Domain.Entities.Course? course = await this._db.CourseRepository.GetAsync(x => x.Id == request.CourseId);
        return course != null;
    }
}