using FluentValidation;

namespace Dissertation.Application.Supervisor.Commands.UpdateResearchArea;

public class UpdateResearchAreaValidator :
    AbstractValidator<UpdateResearchAreaCommand>
{
    public UpdateResearchAreaValidator()
    {
        RuleFor(x => x.ResearchArea)
            .NotEmpty()
            .WithMessage("Research Area is required.");

        RuleFor(x => x.ResearchArea).MaximumLength(1500).WithMessage("Research Area has a limit of 1500 characters");
    }
}