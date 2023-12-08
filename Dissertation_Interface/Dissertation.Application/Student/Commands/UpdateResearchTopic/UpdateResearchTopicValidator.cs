using Dissertation.Application.Supervisor.Commands.UpdateResearchArea;
using FluentValidation;

namespace Dissertation.Application.Student.Commands.UpdateResearchTopic;

public class UpdateResearchTopicValidator :
    AbstractValidator<UpdateResearchTopicCommand>
{
    public UpdateResearchTopicValidator()
    {
        RuleFor(x => x.ResearchTopic)
            .NotEmpty()
            .WithMessage("HTML content is required.");

        RuleFor(x => x.ResearchTopic).MaximumLength(200).WithMessage("Research Topic has a limit of 200 characters");
    }
}