using Dissertation.Application.AcademicYear.Commands.CreateAcademicYear;
using Dissertation.Application.DTO;
using Dissertation.Infrastructure.Persistence.IRepository;
using FluentValidation.TestHelper;
using Moq;
using UnitTests.DissertationAPI.Mocks;

namespace UnitTests.DissertationAPI.Application.AcademicYear.Validators;

public class CreateAcademicYearCommandValidatorTest
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CreateAcademicYearCommandValidator _createAcademicYearCommandValidator;

    public CreateAcademicYearCommandValidatorTest() =>
        this._createAcademicYearCommandValidator = new CreateAcademicYearCommandValidator(
            this._unitOfWork.Object
        );

    [Test]
    public async Task StartDate_ShouldNotBeEmpty()
    {
        // Arrange
        this._unitOfWork
            .Setup(x => x.AcademicYearRepository.IsAcademicYearUnique(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(true);
        var command = new CreateAcademicYearCommand(DateTime.MinValue, DateTime.UtcNow.AddYears(1));

        // Act
        TestValidationResult<CreateAcademicYearCommand>? result = await this._createAcademicYearCommandValidator.TestValidateAsync(command);
        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Any(e => e.ErrorMessage == "Start Date is required"));
        });
    }

    [Test]
    public async Task StartDate_MustBeCurrentOrFutureYear()
    {
        this._unitOfWork
            .Setup(x => x.AcademicYearRepository.IsAcademicYearUnique(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(true);
        var command = new CreateAcademicYearCommand(DateTime.UtcNow.AddYears(-1), DateTime.UtcNow.AddYears(1));

        TestValidationResult<CreateAcademicYearCommand>? result = await this._createAcademicYearCommandValidator.TestValidateAsync(command);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Any(e => e.ErrorMessage == "Start Date must be this year or a future year"));
        });
    }

    [Test]
    public async Task AcademicYear_MustBeUnique()
    {
        this._unitOfWork
            .Setup(x => x.AcademicYearRepository.IsAcademicYearUnique(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(false);
        var command = new CreateAcademicYearCommand(DateTime.UtcNow, DateTime.UtcNow.AddYears(1));

        TestValidationResult<CreateAcademicYearCommand>? result = await this._createAcademicYearCommandValidator.TestValidateAsync(command);

        Assert.IsFalse(result.IsValid);
        Assert.That(result.Errors.Any(e => e.ErrorMessage == "Academic Year with this details exists already"));
    }

    [Test]
    public async Task StartDate_MustBeInSeptember()
    {
        this._unitOfWork
            .Setup(x => x.AcademicYearRepository.IsAcademicYearUnique(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(true);
        var command = new CreateAcademicYearCommand(new DateTime(DateTime.UtcNow.Year, 10, 1), DateTime.UtcNow.AddYears(1));

        TestValidationResult<CreateAcademicYearCommand>? result = await this._createAcademicYearCommandValidator.TestValidateAsync(command);

        Assert.IsFalse(result.IsValid);
        Assert.That(result.Errors.Any(e => e.ErrorMessage == "The Start Month must be September"));
    }

    [Test]
    public async Task EndDate_MustBeGreaterThanStartDate()
    {
        this._unitOfWork
            .Setup(x => x.AcademicYearRepository.IsAcademicYearUnique(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(true);
        var command = new CreateAcademicYearCommand(DateTime.UtcNow, DateTime.UtcNow.AddDays(-1));

        TestValidationResult<CreateAcademicYearCommand>? result = await this._createAcademicYearCommandValidator.TestValidateAsync(command);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Any(e => e.ErrorMessage == "End Date must be a more recent date than the Start Date"));
        });
    }

    [Test]
    public async Task EndDate_MustBeOneYearAfterStartDate()
    {
        this._unitOfWork
            .Setup(x => x.AcademicYearRepository.IsAcademicYearUnique(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(true);
        var command = new CreateAcademicYearCommand(DateTime.UtcNow, DateTime.UtcNow);

        TestValidationResult<CreateAcademicYearCommand>? result = await this._createAcademicYearCommandValidator.TestValidateAsync(command);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Any(e => e.ErrorMessage == "End Date must be Start Date plus one year"));
        });
    }

    [Test]
    public async Task EndDate_MustBeInAugust()
    {
        this._unitOfWork
            .Setup(x => x.AcademicYearRepository.IsAcademicYearUnique(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(true);
        var command = new CreateAcademicYearCommand(new DateTime(DateTime.UtcNow.Year, 9, 1), new DateTime(DateTime.UtcNow.Year + 1, 9, 1));

        TestValidationResult<CreateAcademicYearCommand>? result = await this._createAcademicYearCommandValidator.TestValidateAsync(command);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Any(e => e.ErrorMessage == "The End Month must be August"));
        });
    }

    [Test]
    public async Task EndDate_ShouldNotBeEmpty()
    {
        this._unitOfWork
            .Setup(x => x.AcademicYearRepository.IsAcademicYearUnique(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(true);
        var command = new CreateAcademicYearCommand(DateTime.UtcNow, DateTime.MinValue);

        TestValidationResult<CreateAcademicYearCommand>? result = await this._createAcademicYearCommandValidator.TestValidateAsync(command);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Any(e => e.ErrorMessage == "End Date is required"));
        });
    }

    [Test]
    public async Task AllConditions_Valid_ShouldPassValidation()
    {
        // Arrange
        this._unitOfWork
            .Setup(x => x.AcademicYearRepository.IsAcademicYearUnique(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(true);

        var currentYear = DateTime.UtcNow.Year;
        var startDate = new DateTime(currentYear, 9, 1); // September of the current year
        var endDate = new DateTime(currentYear + 1, 8, 31); // August of the next year

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        // Set up the mock to return true for the unique academic year check
        mockUnitOfWork.Setup(m => m.AcademicYearRepository.IsAcademicYearUnique(startDate, endDate)).ReturnsAsync(true);

        var validator = new CreateAcademicYearCommandValidator(mockUnitOfWork.Object);
        var command = new CreateAcademicYearCommand(startDate, endDate);

        // Act
        TestValidationResult<CreateAcademicYearCommand>? result = await this._createAcademicYearCommandValidator.TestValidateAsync(command);
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsValid, Is.True); // The validation should pass
            Assert.That(result.Errors, Is.Empty); // There should be no validation errors
        });
    }
}