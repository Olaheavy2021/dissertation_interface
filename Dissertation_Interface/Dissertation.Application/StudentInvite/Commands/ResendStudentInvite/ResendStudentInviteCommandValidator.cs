using FluentValidation;
using Shared.Constants;

namespace Dissertation.Application.StudentInvite.Commands.ResendStudentInvite;

public class ResendStudentInviteCommandValidator : AbstractValidator<ResendStudentInviteCommand>
{
    public ResendStudentInviteCommandValidator()
    {
        RuleFor(p => p.StudentId)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace);

        RuleFor(p => p.InvitationCode)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField);
    }
}