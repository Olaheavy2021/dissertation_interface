using FluentValidation;
using Shared.Constants;

namespace Dissertation.Application.Supervisor.Commands.AcceptSupervisionRequest;

public class AcceptSupervisionRequestCommandValidator : AbstractValidator<AcceptSupervisionRequestCommand>
{
    public AcceptSupervisionRequestCommandValidator()
    {
        RuleFor(x => x.RequestId).NotEmpty().WithMessage(ErrorMessages.RequiredField);
        RuleFor(x => x.Comment).NotEmpty().WithMessage(ErrorMessages.RequiredField);
        RuleFor(x => x.Comment).MaximumLength(500).WithMessage("Comment can not be more than 500 characters");
    }
}