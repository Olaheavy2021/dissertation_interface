using FluentValidation;
using Shared.Constants;

namespace Dissertation.Application.SupervisorInvite.Commands.ResendSupervisorInvite;

public class ResendSupervisorInviteCommandValidator : AbstractValidator<ResendSupervisorInviteCommand>
{
    public ResendSupervisorInviteCommandValidator() =>
        RuleFor(p => p.InviteId)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField);
}