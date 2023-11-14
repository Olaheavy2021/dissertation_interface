using Dissertation.Infrastructure.Persistence.IRepository;
using FluentValidation;
using Shared.Exceptions;

namespace Dissertation.Application.AcademicYear.Commands.UpdateAcademicYear;

public class UpdateAcademicYearCommandValidator : AbstractValidator<UpdateAcademicYearCommand>
{
    private readonly IUnitOfWork _db;

    public UpdateAcademicYearCommandValidator(IUnitOfWork db)
    {
        this._db = db;
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start Date is required");
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End Date is required");
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Unique Identifier for the Academic Year can not be null");
        RuleFor(q => q)
            .MustAsync(IsStartDateTheSame).WithMessage("Start Date cannot be updated to a different year")
            .OverridePropertyName("StartDate");
        RuleFor(q => q)
            .MustAsync(IsEndDateTheSame).WithMessage("End Date cannot be updated to a different year")
            .OverridePropertyName("EndDate");
    }

    private async Task<bool> IsStartDateTheSame(UpdateAcademicYearCommand request, CancellationToken token)
    {
        Domain.Entities.AcademicYear? academicYear = await this._db.AcademicYearRepository.GetFirstOrDefaultAsync(a => a.Id == request.Id);
        if (academicYear == null)
        {
            throw new NotFoundException(nameof(AcademicYear), request.Id);
        }

        if (!academicYear.StartDate.Date.Year.Equals(request.StartDate.Date.Year))
        {
            return false;
        }

        return true;
    }

    private async Task<bool> IsEndDateTheSame(UpdateAcademicYearCommand request, CancellationToken token)
    {
        Domain.Entities.AcademicYear? academicYear = await this._db.AcademicYearRepository.GetFirstOrDefaultAsync(a => a.Id == request.Id);
        if (academicYear == null)
        {
            throw new NotFoundException(nameof(AcademicYear), request.Id);
        }

        if (!academicYear.EndDate.Date.Year.Equals(request.EndDate.Date.Year))
        {
            return false;
        }

        return true;
    }

}