using FluentValidation;

namespace Dissertation.Application.Student.Commands.InitiateSupervisionRequest;

public class InitiateSupervisionRequestCommandValidator : AbstractValidator<InitiateSupervisionRequestCommand>
{
    public InitiateSupervisionRequestCommandValidator() => RuleFor(x => x.SupervisorId).NotEmpty();
}