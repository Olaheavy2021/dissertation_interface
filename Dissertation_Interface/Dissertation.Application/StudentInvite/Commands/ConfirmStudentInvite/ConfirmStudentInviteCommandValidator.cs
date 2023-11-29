using Dissertation.Application.SupervisorInvite.Commands.ConfirmSupervisorInvite;
using FluentValidation;
using Shared.Constants;

namespace Dissertation.Application.StudentInvite.Commands.ConfirmStudentInvite;

public class ConfirmStudentInviteCommandValidator : AbstractValidator<ConfirmStudentInviteCommand>
{
    public ConfirmStudentInviteCommandValidator()
    {
        RuleFor(p => p.StudentId)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace);

        RuleFor(p => p.InvitationCode)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField);
    }
}