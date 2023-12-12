using FluentValidation;
using Shared.Constants;

namespace Dissertation.Application.Student.Commands.CancelSupervisionRequest;

public class CancelSupervisionRequestCommandValidator : AbstractValidator<CancelSupervisionRequestCommand>
{
    public CancelSupervisionRequestCommandValidator()
    {
        RuleFor(x => x.RequestId).NotEmpty().WithMessage(ErrorMessages.RequiredField);
        RuleFor(x => x.Comment).NotEmpty().WithMessage(ErrorMessages.RequiredField);
        RuleFor(x => x.Comment).MaximumLength(500).WithMessage("Comment can not be more than 500 characters");
    }
}