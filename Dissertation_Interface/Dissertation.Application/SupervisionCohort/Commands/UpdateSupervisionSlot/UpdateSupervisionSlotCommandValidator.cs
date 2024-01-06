using FluentValidation;

namespace Dissertation.Application.SupervisionCohort.Commands.UpdateSupervisionSlot;

public class UpdateSupervisionSlotCommandValidator : AbstractValidator<UpdateSupervisionSlotCommand>
{
    public UpdateSupervisionSlotCommandValidator() => RuleFor(x => x.SupervisionCohortId).GreaterThan(0);
}