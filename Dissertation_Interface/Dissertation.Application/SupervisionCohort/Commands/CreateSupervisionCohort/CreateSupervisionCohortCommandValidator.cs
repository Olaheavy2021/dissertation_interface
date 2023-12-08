using FluentValidation;

namespace Dissertation.Application.SupervisionCohort.Commands.CreateSupervisionCohort;

public class CreateSupervisionCohortCommandValidator : AbstractValidator<CreateSupervisionCohortCommand>
{
    public CreateSupervisionCohortCommandValidator() => RuleForEach(x => x.SupervisionCohortRequests).SetValidator(new CreateSupervisionCohortRequestValidator());
}