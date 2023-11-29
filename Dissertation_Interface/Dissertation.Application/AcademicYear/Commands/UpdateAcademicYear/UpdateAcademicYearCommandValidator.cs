using Dissertation.Application.Utility;
using Dissertation.Infrastructure.Persistence.IRepository;
using FluentValidation;

namespace Dissertation.Application.AcademicYear.Commands.UpdateAcademicYear;

public class  UpdateAcademicYearCommandValidator : AbstractValidator<UpdateAcademicYearCommand>
{
    private readonly IUnitOfWork _db;

    public UpdateAcademicYearCommandValidator(IUnitOfWork db)
    {
        this._db = db;
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start Date is required");
        RuleFor(x => x.StartDate)
            .Must(x => x.Date.Year >= DateTime.UtcNow.Date.Year)
            .NotEmpty().WithMessage("Start Date must be this year or a future year");
        RuleFor(q => q)
            .Must(IsStartDateSeptember).WithMessage("The Start Month must be September")
            .OverridePropertyName("StartDate");
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End Date is required");
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Unique Identifier for the Academic Year can not be empty");
        RuleFor(q => q)
            .Must(IsEndDateGreaterThanStartDate).WithMessage("End Date must be a more recent date than the Start Date")
            .OverridePropertyName("EndDate");
        RuleFor(q => q)
            .Must(IsEndDateTheNextYear).WithMessage("End Date must be Start Date plus one year")
            .OverridePropertyName("EndDate");
        RuleFor(q => q)
            .MustAsync(IsAcademicYearUnique).WithMessage($"Academic Year already exists")
            .OverridePropertyName("StartDate")
            .OverridePropertyName("EndDate");
        RuleFor(q => q)
            .MustAsync(IsDissertationCohortValid).WithMessage($"Academic Year Start and End Date must match the dissertation cohorts already created ")
            .OverridePropertyName("StartDate")
            .OverridePropertyName("EndDate");
        RuleFor(q => q)
            .Must(IsEndDateJuly).WithMessage("The End Month must be August")
            .OverridePropertyName("EndDate");
    }

    private async Task<bool> IsAcademicYearUnique(UpdateAcademicYearCommand request, CancellationToken token)
    {
        Domain.Entities.AcademicYear? academicYearFromDb = await this._db.AcademicYearRepository.GetFirstOrDefaultAsync(x => x.Id == request.Id);
        if (academicYearFromDb != null && academicYearFromDb.StartDate.Date.Year == request.StartDate.Date.Year &&
            academicYearFromDb.EndDate.Date.Year == request.EndDate.Date.Year)
        {
            return true;
        }

        return await this._db.AcademicYearRepository.IsAcademicYearUnique(request.StartDate, request.EndDate);
    }

    private static bool IsEndDateGreaterThanStartDate(UpdateAcademicYearCommand request) => request.StartDate.Date < request.EndDate.Date;

    private static bool IsEndDateTheNextYear(UpdateAcademicYearCommand request) =>
        request.EndDate.Date.Year - request.StartDate.Date.Year == 1;

    private async Task<bool> IsDissertationCohortValid(UpdateAcademicYearCommand request,
        CancellationToken token)
    {
        IReadOnlyList<Domain.Entities.DissertationCohort> dissertationCohorts =
            await this._db.DissertationCohortRepository.GetAllAsync(a => a.AcademicYearId == request.Id);

        if (!dissertationCohorts.Any())
        {
            return true;
        }

        IOrderedEnumerable<Domain.Entities.DissertationCohort> orderByDescendingEndDate = dissertationCohorts.OrderByDescending(x => x.EndDate);
        DateTime endDate = orderByDescendingEndDate.First().EndDate;

        IOrderedEnumerable<Domain.Entities.DissertationCohort> orderByAscendingStartDate = dissertationCohorts.OrderBy(x => x.StartDate);
        DateTime startDate = orderByAscendingStartDate.First().StartDate;

        if (request.StartDate > startDate)
        {
            return false;
        }

        return request.EndDate <  endDate;
    }

    private static bool IsStartDateSeptember(UpdateAcademicYearCommand request) =>
        request.StartDate.Date.Month == MonthConstants.MonthConstantSeptember;

    private static bool IsEndDateJuly(UpdateAcademicYearCommand request) =>
        request.EndDate.Date.Month == MonthConstants.MonthConstantAugust;
}