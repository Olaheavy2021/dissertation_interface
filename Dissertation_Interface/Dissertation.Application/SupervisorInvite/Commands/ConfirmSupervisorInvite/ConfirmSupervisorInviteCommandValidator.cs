﻿using FluentValidation;
using Shared.Constants;

namespace Dissertation.Application.SupervisorInvite.Commands.ConfirmSupervisorInvite;

public class ConfirmSupervisorInviteCommandValidator : AbstractValidator<ConfirmSupervisorInviteCommand>
{
    public ConfirmSupervisorInviteCommandValidator()
    {
        RuleFor(p => p.StaffId)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .NotNull().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace);

        RuleFor(p => p.InvitationCode)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .NotNull().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(8).WithMessage("Invitation Code is more than the Maximum Length")
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace);
    }
}