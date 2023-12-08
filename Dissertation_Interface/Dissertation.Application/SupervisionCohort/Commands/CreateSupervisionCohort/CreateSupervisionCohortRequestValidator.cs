using FluentValidation;
using Shared.DTO;

namespace Dissertation.Application.SupervisionCohort.Commands.CreateSupervisionCohort;

public class CreateSupervisionCohortRequestValidator :  AbstractValidator<CreateSupervisionCohortRequest>
{
    public CreateSupervisionCohortRequestValidator()
    {
        RuleFor(request => request.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");

        RuleFor(request => request.SupervisionSlot)
            .GreaterThanOrEqualTo(1)
            .WithMessage("SupervisionSlot must be greater than or equal to 1.");
    }
}