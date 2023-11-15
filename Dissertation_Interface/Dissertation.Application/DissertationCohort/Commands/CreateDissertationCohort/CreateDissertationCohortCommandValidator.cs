using Dissertation.Domain.Enums;
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
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End Date is required");
        RuleFor(x => x.SupervisionChoiceDeadline)
            .NotEmpty().WithMessage("Supervision Deadline is required");
        RuleFor(x => x.StartDate).LessThan(x => x.EndDate)
            .WithMessage("Start Date should be an earlier date than the End Date");
        RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate)
            .WithMessage("End Date should be a later date to the Start Date");
        RuleFor(x => x.SupervisionChoiceDeadline)
            .NotEmpty().WithMessage("Supervision Deadline is required")
            .Must((model, deadline) => deadline > model.StartDate && deadline < model.EndDate)
            .WithMessage("Supervision Choice Deadline must be between the Start Date and End Date");
        RuleFor(q => q)
            .MustAsync(IsStartDateWithinAcademicSession).WithMessage("Start Date does not fall within the current academic year")
            .OverridePropertyName("StartDate");
        RuleFor(q => q)
            .MustAsync(IsEndDateWithinAcademicSession).WithMessage("End Date does not fall within the current academic year")
            .OverridePropertyName("EndDate");
        RuleFor(q => q)
            .MustAsync(IsAcademicYearActive).WithMessage("Academic year is not active")
            .OverridePropertyName("AcademicYearId");
        RuleFor(q => q)
            .MustAsync(IsDissertationCohortMoreThanThree)
            .WithMessage("Dissertation Cohorts for an Academic Year can not be more than 3")
            .OverridePropertyName("AcademicYearId");
    }

    private async Task<bool> IsStartDateWithinAcademicSession(CreateDissertationCohortCommand request, CancellationToken token)
    {
        Domain.Entities.AcademicYear? academicYear =
            await this._db.AcademicYearRepository.GetFirstOrDefaultAsync(a => a.Id == request.AcademicYearId);

        if (academicYear == null)
        {
            return false;
        }

        return request.StartDate.Date >= academicYear.StartDate.Date && request.StartDate.Date < academicYear.EndDate.Date;
    }

    private async Task<bool> IsEndDateWithinAcademicSession(CreateDissertationCohortCommand request, CancellationToken token)
    {
        Domain.Entities.AcademicYear? academicYear =
            await this._db.AcademicYearRepository.GetFirstOrDefaultAsync(a => a.Id == request.AcademicYearId);

        if (academicYear == null)
        {
            return false;
        }

        return request.EndDate.Date >= academicYear.StartDate.Date && request.EndDate.Date <= academicYear.EndDate.Date;
    }

    private async Task<bool> IsAcademicYearActive(CreateDissertationCohortCommand request, CancellationToken token)
    {
        Domain.Entities.AcademicYear? academicYear =
            await this._db.AcademicYearRepository.GetFirstOrDefaultAsync(a => a.Id == request.AcademicYearId);
        return academicYear != null && academicYear.Status.Equals(DissertationConfigStatus.Active);
    }

    private async Task<bool> IsDissertationCohortMoreThanThree(CreateDissertationCohortCommand request,
        CancellationToken token)
    {
        IReadOnlyList<Domain.Entities.DissertationCohort> dissertationCohorts =
            await this._db.DissertationCohortRepository.GetAllAsync(a => a.AcademicYearId == request.AcademicYearId);

        return dissertationCohorts.Count <= 2;
    }

}