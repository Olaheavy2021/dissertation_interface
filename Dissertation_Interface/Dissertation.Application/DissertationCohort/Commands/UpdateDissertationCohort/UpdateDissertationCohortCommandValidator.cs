using Dissertation.Application.Utility;
using Dissertation.Infrastructure.Persistence.IRepository;
using FluentValidation;

namespace Dissertation.Application.DissertationCohort.Commands.UpdateDissertationCohort;

public class UpdateDissertationCohortCommandValidator : AbstractValidator<UpdateDissertationCohortCommand>
{
    private readonly IUnitOfWork _db;

    public UpdateDissertationCohortCommandValidator(IUnitOfWork db)
    {
        this._db = db;
        RuleFor(x => x.StartDate).NotEmpty().WithMessage("Start Date is required");
        RuleFor(x => x.StartDate).LessThan(x => x.EndDate)
            .WithMessage("Start Date should be an earlier date than the End Date");
        RuleFor(x => x)
            .MustAsync(IsStartDateValid)
            .WithMessage("Start Month for a Cohort cannot be modified to a different month")
            .OverridePropertyName("StartDate");
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End Date is required");
        RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate)
            .WithMessage("End Date should be a later date to the Start Date");
        RuleFor(x => x)
            .MustAsync(IsEndDateValid)
            .WithMessage("End Date for a Cohort cannot be modified to a different month")
            .OverridePropertyName("EndDate");
        RuleFor(x => x.SupervisionChoiceDeadline)
            .NotEmpty().WithMessage("Supervision Deadline is required");
        RuleFor(x => x.SupervisionChoiceDeadline)
            .Must((model, deadline) => deadline > model.StartDate && deadline < model.EndDate)
            .WithMessage("Supervision Choice Deadline must be between the Start Date and End Date");
    }

    private async Task<bool> IsStartDateValid(UpdateDissertationCohortCommand request, CancellationToken token)
    {
        //must be either September and the start year of the session for the first one
        //must be either May and the end year of the session for the second one

        Domain.Entities.DissertationCohort? dissertationCohort =
            await this._db.DissertationCohortRepository.GetFirstOrDefaultAsync(a => a.Id == request.Id, includes: x => x.AcademicYear);

        if (dissertationCohort != null)
        {
            if (dissertationCohort.StartDate.Month == MonthConstants.MonthConstantSeptember)
            {
                return request.StartDate.Date.Month == MonthConstants.MonthConstantSeptember && request.StartDate.Year == dissertationCohort.AcademicYear.StartDate.Year && request.StartDate >= dissertationCohort.AcademicYear.StartDate;
            }

            if (dissertationCohort.StartDate.Month == MonthConstants.MonthConstantMay)
            {
                return request.StartDate.Date.Month == MonthConstants.MonthConstantMay && request.StartDate.Year == dissertationCohort.AcademicYear.EndDate.Year;
            }

            return false;
        }

        return false;
    }


    private async Task<bool> IsEndDateValid(UpdateDissertationCohortCommand request, CancellationToken token)
    {
        //must be either January and the end year of the session for the first one
        //must be either August and the end year of the session for the second one

        Domain.Entities.DissertationCohort? dissertationCohort =
            await this._db.DissertationCohortRepository.GetFirstOrDefaultAsync(a => a.Id == request.Id, includes: x => x.AcademicYear);

        if (dissertationCohort != null)
        {
            if (dissertationCohort.EndDate.Month == MonthConstants.MonthConstantJanuary)
            {
                return request.EndDate.Date.Month == MonthConstants.MonthConstantJanuary && request.EndDate.Year == dissertationCohort.AcademicYear.EndDate.Year && request.EndDate.Date <= dissertationCohort.AcademicYear.EndDate.Date;
            }

            if (dissertationCohort.EndDate.Month == MonthConstants.MonthConstantAugust)
            {
                return request.EndDate.Date.Month == MonthConstants.MonthConstantAugust && request.EndDate.Year == dissertationCohort.AcademicYear.EndDate.Year && request.EndDate.Date <= dissertationCohort.AcademicYear.EndDate.Date;
            }

            return false;
        }

        return false;
    }
}