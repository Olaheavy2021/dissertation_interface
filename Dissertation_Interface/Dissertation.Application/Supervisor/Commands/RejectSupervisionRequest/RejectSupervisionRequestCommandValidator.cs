using FluentValidation;
using Shared.Constants;

namespace Dissertation.Application.Supervisor.Commands.RejectSupervisionRequest;

public class RejectSupervisionRequestCommandValidator : AbstractValidator<RejectSupervisionRequestCommand>
{
    public RejectSupervisionRequestCommandValidator()
    {
        RuleFor(x => x.RequestId).NotEmpty().WithMessage(ErrorMessages.RequiredField);
        RuleFor(x => x.Comment).NotEmpty().WithMessage(ErrorMessages.RequiredField);
        RuleFor(x => x.Comment).MaximumLength(500).WithMessage("Comment can not be more than 500 characters");
    }
}