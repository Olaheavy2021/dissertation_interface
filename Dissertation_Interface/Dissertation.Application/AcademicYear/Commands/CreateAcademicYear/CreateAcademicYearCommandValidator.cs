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
            .NotEmpty().WithMessage("Start Date is required")
            .Must(date => date.Year == DateTime.UtcNow.Year).WithMessage("Start Date must be the current year");
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End Date is required")
            .Must(date => date.Year == DateTime.UtcNow.Year + 1).WithMessage("End Date must be the current year plus one");
        RuleFor(q => q)
            .MustAsync(IsAcademicYearUnique).WithMessage("Academic Year with this details exists already")
            .OverridePropertyName("StartDate");
    }

    private async Task<bool> IsAcademicYearUnique(CreateAcademicYearCommand request, CancellationToken token) =>
        await this._db.AcademicYearRepository.IsAcademicYearUnique(request.StartDate, request.EndDate);
}