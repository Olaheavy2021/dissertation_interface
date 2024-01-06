using FluentValidation;
using Shared.Constants;
using Shared.DTO;
using UserManagement_API.Data.Models.Dto;

namespace UserManagement_API.Data.Models.Validators;

public class EditStudentRequestDtoValidator : AbstractValidator<EditStudentRequestDto>
{
    public EditStudentRequestDtoValidator()
    {
        RuleFor(p => p.UserId)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField);

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

        RuleFor(p => p.StudentId)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .NotNull().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace);

        RuleFor(p => p.CourseId)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField);
    }

}