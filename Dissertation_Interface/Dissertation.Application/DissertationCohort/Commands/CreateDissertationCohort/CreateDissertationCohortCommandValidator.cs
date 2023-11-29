using Dissertation.Application.Utility;
using Dissertation.Infrastructure.Persistence.IRepository;
using FluentValidation;

namespace Dissertation.Application.DissertationCohort.Commands.CreateDissertationCohort;

public class CreateDissertationCohortCommandValidator : AbstractValidator<CreateDissertationCohortCommand>
{
    private readonly IUnitOfWork _db;

    public CreateDissertationCohortCommandValidator(IUnitOfWork db)
    {
        this._db = db;
        RuleFor(x => x.StartDate).NotEmpty().WithMessage("Start Date is required");
        RuleFor(x => x.StartDate).LessThan(x => x.EndDate)
            .WithMessage("Start Date should be an earlier date than the End Date");
        RuleFor(x => x)
            .MustAsync(IsStartDateValid)
            .WithMessage("Start Month for a Cohort is either September (First Cohort) or May (Second Cohort) and within the Academic Year")
            .OverridePropertyName("StartDate");
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End Date is required");
        RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate)
            .WithMessage("End Date should be a later date to the Start Date");
        RuleFor(x => x)
            .MustAsync(IsEndDateValid)
            .WithMessage("End Month for a Cohort is either January (First Cohort) or August (Second Cohort) and within the Academic Year")
            .OverridePropertyName("EndDate");
        RuleFor(q => q)
            .MustAsync(IsDissertationCohortMoreThanTwo)
            .WithMessage("Dissertation Cohorts for an Academic Year can not be more than 2")
            .OverridePropertyName("AcademicYearId");
        RuleFor(x => x.SupervisionChoiceDeadline)
            .NotEmpty().WithMessage("Supervision Deadline is required");
        RuleFor(x => x.SupervisionChoiceDeadline)
            .Must((model, deadline) => deadline > model.StartDate && deadline < model.EndDate)
            .WithMessage("Supervision Choice Deadline must be between the Start Date and End Date");
    }

    private async Task<bool> IsDissertationCohortMoreThanTwo(CreateDissertationCohortCommand request,
        CancellationToken token)
    {
        IReadOnlyList<Domain.Entities.DissertationCohort> dissertationCohorts =
            await this._db.DissertationCohortRepository.GetAllAsync(a => a.AcademicYearId == request.AcademicYearId);

        return dissertationCohorts.Count <= 1;
    }

    private async Task<bool> IsStartDateValid(CreateDissertationCohortCommand request, CancellationToken token)
    {
        //must be either September and the start year of the session for the first one
        //must be either May and the end year of the session for the second one
        var doesCohortExist =
            await this._db.DissertationCohortRepository.AnyAsync(a => a.AcademicYearId == request.AcademicYearId);

        Domain.Entities.AcademicYear? academicYear =
            await this._db.AcademicYearRepository.GetFirstOrDefaultAsync(a => a.Id == request.AcademicYearId);

        if (!doesCohortExist)
        {
            return request.StartDate.Date.Month == MonthConstants.MonthConstantSeptember && academicYear != null && request.StartDate.Year == academicYear.StartDate.Year && request.StartDate.Date >= academicYear.StartDate.Date;
        }

        return request.StartDate.Date.Month == MonthConstants.MonthConstantMay && academicYear != null && request.StartDate.Year == academicYear.EndDate.Year;
    }


    private async Task<bool> IsEndDateValid(CreateDissertationCohortCommand request, CancellationToken token)
    {
        //must be either January and the end year of the session for the first one
        //must be either August and the end year of the session for the second one
        var doesCohortExist =
            await this._db.DissertationCohortRepository.AnyAsync(a => a.AcademicYearId == request.AcademicYearId);

        Domain.Entities.AcademicYear? academicYear =
            await this._db.AcademicYearRepository.GetFirstOrDefaultAsync(a => a.Id == request.AcademicYearId);

        if (!doesCohortExist)
        {
            return request.EndDate.Date.Month == MonthConstants.MonthConstantJanuary && academicYear != null && request.EndDate.Year == academicYear.EndDate.Year && request.EndDate.Date <= academicYear.EndDate.Date;
        }

        return request.EndDate.Date.Month == MonthConstants.MonthConstantAugust && academicYear != null && request.EndDate.Year == academicYear.EndDate.Year && request.EndDate.Date <= academicYear.EndDate.Date;
    }
}