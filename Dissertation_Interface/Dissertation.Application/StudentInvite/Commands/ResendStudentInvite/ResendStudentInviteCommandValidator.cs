using FluentValidation;
using Shared.Constants;

namespace Dissertation.Application.StudentInvite.Commands.ResendStudentInvite;

public class ResendStudentInviteCommandValidator : AbstractValidator<ResendStudentInviteCommand>
{
    public ResendStudentInviteCommandValidator() =>
        RuleFor(p => p.InviteId)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField);
}