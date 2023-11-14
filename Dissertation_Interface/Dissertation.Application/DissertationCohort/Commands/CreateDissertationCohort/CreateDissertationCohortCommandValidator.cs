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
        RuleFor(x => x.StartDate).GreaterThan(x => x.EndDate)
            .WithMessage("StartDate should be an earlier date than the End Date");
        RuleFor(x => x.EndDate).LessThan(x => x.StartDate)
            .WithMessage("End should be a later date to the Start Date");
        RuleFor(q => q)
            .MustAsync(IsStartDateWithinAcademicSession).WithMessage("Start Date does not fall within the current academic year or ")
            .OverridePropertyName("StartDate");
        RuleFor(q => q)
            .MustAsync(IsEndDateWithinAcademicSession).WithMessage("End Date does not fall within the current academic year")
            .OverridePropertyName("EndDate");
        RuleFor(q => q)
            .MustAsync(IsAcademicYearActive).WithMessage("Academic year is not active")
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

        if (request.StartDate < academicYear.StartDate || request.StartDate > academicYear.EndDate)
        {
            return false;
        }

        return true;
    }

    private async Task<bool> IsEndDateWithinAcademicSession(CreateDissertationCohortCommand request, CancellationToken token)
    {
        Domain.Entities.AcademicYear? academicYear =
            await this._db.AcademicYearRepository.GetFirstOrDefaultAsync(a => a.Id == request.AcademicYearId);

        if (academicYear == null)
        {
            return false;
        }

        if (request.EndDate < academicYear.StartDate || request.EndDate > academicYear.EndDate)
        {
            return false;
        }

        return true;
    }

    private async Task<bool> IsAcademicYearActive(CreateDissertationCohortCommand request, CancellationToken token)
    {
        Domain.Entities.AcademicYear? academicYear =
            await this._db.AcademicYearRepository.GetFirstOrDefaultAsync(a => a.Id == request.AcademicYearId);
        if (academicYear == null)
        {
            return false;
        }

        if (academicYear.Status.Equals(DissertationConfigStatus.Active))
        {
            return true;
        }

        return false;
    }

}