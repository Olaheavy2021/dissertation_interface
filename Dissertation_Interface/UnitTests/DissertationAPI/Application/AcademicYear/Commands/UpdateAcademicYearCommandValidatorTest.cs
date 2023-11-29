using Dissertation.Application.AcademicYear.Commands.UpdateAcademicYear;
using Dissertation.Infrastructure.Persistence.IRepository;
using Moq;

namespace UnitTests.DissertationAPI.Application.AcademicYear.Commands;

public class UpdateAcademicYearCommandValidatorTest
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly UpdateAcademicYearCommandValidator _updateAcademicYearCommandValidator;

    public UpdateAcademicYearCommandValidatorTest() =>
        this._updateAcademicYearCommandValidator = new UpdateAcademicYearCommandValidator(
            this._unitOfWork.Object
        );
}