using FluentValidation;
using Shared.Constants;

namespace Dissertation.Application.SupervisorInvite.Commands.ResendSupervisorInvite;

public class ResendSupervisorInviteCommandValidator : AbstractValidator<ResendSupervisorInviteCommand>
{
    public ResendSupervisorInviteCommandValidator()
    {
        RuleFor(p => p.StaffId)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace);

        RuleFor(p => p.InvitationCode)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField);
    }
}