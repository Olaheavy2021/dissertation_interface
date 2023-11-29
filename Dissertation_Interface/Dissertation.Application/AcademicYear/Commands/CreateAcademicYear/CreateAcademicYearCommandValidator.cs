using Dissertation.Application.Utility;
using Dissertation.Infrastructure.Persistence.IRepository;
using FluentValidation;

namespace Dissertation.Application.AcademicYear.Commands.CreateAcademicYear;

public class CreateAcademicYearCommandValidator : AbstractValidator<CreateAcademicYearCommand>
{
    private readonly IUnitOfWork _db;
    public CreateAcademicYearCommandValidator(IUnitOfWork db)
    {
        this._db = db;
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start Date is required");
        RuleFor(x => x.StartDate)
            .Must(x => x.Date.Year >= DateTime.UtcNow.Date.Year)
            .WithMessage("Start Date must be this year or a future year");
        RuleFor(q => q)
            .MustAsync(IsAcademicYearUnique).WithMessage("Academic Year with this details exists already")
            .OverridePropertyName("StartDate");
        RuleFor(q => q)
            .Must(IsStartDateSeptember).WithMessage("The Start Month must be September")
            .OverridePropertyName("StartDate");
        RuleFor(q => q)
            .Must(IsEndDateGreaterThanStartDate).WithMessage("End Date must be a more recent date than the Start Date")
            .OverridePropertyName("EndDate");
        RuleFor(q => q)
            .Must(IsEndDateTheNextYear).WithMessage("End Date must be Start Date plus one year")
            .OverridePropertyName("EndDate");
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End Date is required");
        RuleFor(q => q)
            .Must(IsEndDateAugust).WithMessage("The End Month must be August")
            .OverridePropertyName("EndDate");
    }

    private async Task<bool> IsAcademicYearUnique(CreateAcademicYearCommand request, CancellationToken token) =>
        await this._db.AcademicYearRepository.IsAcademicYearUnique(request.StartDate, request.EndDate);

    private static bool IsEndDateGreaterThanStartDate(CreateAcademicYearCommand request) => request.StartDate.Date < request.EndDate.Date;

    private static bool IsEndDateTheNextYear(CreateAcademicYearCommand request) =>
       request.EndDate.Date.Year - request.StartDate.Date.Year == 1;

    private static bool IsStartDateSeptember(CreateAcademicYearCommand request) =>
        request.StartDate.Date.Month == MonthConstants.MonthConstantSeptember;

    private static bool IsEndDateAugust(CreateAcademicYearCommand request) =>
        request.EndDate.Date.Month == MonthConstants.MonthConstantAugust;
}