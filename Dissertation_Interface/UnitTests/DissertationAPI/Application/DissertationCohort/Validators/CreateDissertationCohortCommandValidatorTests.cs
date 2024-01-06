using System.Linq.Expressions;
using Dissertation.Application.DissertationCohort.Commands.CreateDissertationCohort;
using Dissertation.Infrastructure.Persistence.IRepository;
using FluentValidation.TestHelper;
using Moq;
using UnitTests.DissertationAPI.Mocks;

namespace UnitTests.DissertationAPI.Application.DissertationCohort.Validators;

public class CreateDissertationCohortCommandValidatorTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CreateDissertationCohortCommandValidator _createDissertationCohortCommandValidator;

    public CreateDissertationCohortCommandValidatorTests() =>
        this._createDissertationCohortCommandValidator = new CreateDissertationCohortCommandValidator(
            this._unitOfWork.Object
        );

    [Test]
    public async Task StartDate_ShouldNotBeEmpty()
    {
        // Arrange
        this._unitOfWork
            .Setup(x => x.AcademicYearRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Dissertation.Domain.Entities.AcademicYear, bool>>>(), It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.AcademicYear>, IOrderedQueryable<Dissertation.Domain.Entities.AcademicYear>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.AcademicYear, object>>[]>()))
            .ReturnsAsync(AcademicYearMocks.GetFirstOrDefaultResponse());

        this._unitOfWork
            .Setup(x => x.DissertationCohortRepository.GetAllAsync(It.IsAny<Expression<Func<Dissertation.Domain.Entities.DissertationCohort, bool>>>(), It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.DissertationCohort>, IOrderedQueryable<Dissertation.Domain.Entities.DissertationCohort>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.DissertationCohort, object>>[]>()))
            .ReturnsAsync(DissertationCohortMocks.GetPaginatedResponse);

        var command = new CreateDissertationCohortCommand(DateTime.MinValue, DateTime.UtcNow.AddDays(20), DateTime.UtcNow.AddDays(30), 2);

        // Act
        TestValidationResult<CreateDissertationCohortCommand>? result = await this._createDissertationCohortCommandValidator.TestValidateAsync(command);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.That(result.Errors.Any(e => e.ErrorMessage == "Start Date is required"));
    }

    [Test]
    public async Task StartDate_ShouldBeEarlierThanEndDate()
    {
        this._unitOfWork
            .Setup(x => x.AcademicYearRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Dissertation.Domain.Entities.AcademicYear, bool>>>(), It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.AcademicYear>, IOrderedQueryable<Dissertation.Domain.Entities.AcademicYear>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.AcademicYear, object>>[]>()))
            .ReturnsAsync(AcademicYearMocks.GetFirstOrDefaultResponse());

        this._unitOfWork
            .Setup(x => x.DissertationCohortRepository.AnyAsync(It.IsAny<Expression<Func<Dissertation.Domain.Entities.DissertationCohort, bool>>>()))
            .ReturnsAsync(true);

        this._unitOfWork
            .Setup(x => x.DissertationCohortRepository.GetAllAsync(It.IsAny<Expression<Func<Dissertation.Domain.Entities.DissertationCohort, bool>>>(), It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.DissertationCohort>, IOrderedQueryable<Dissertation.Domain.Entities.DissertationCohort>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.DissertationCohort, object>>[]>()))
            .ReturnsAsync(DissertationCohortMocks.GetPaginatedResponse);
        var command = new CreateDissertationCohortCommand(DateTime.UtcNow.AddDays(1), DateTime.UtcNow, DateTime.UtcNow.AddDays(30), 2);

        TestValidationResult<CreateDissertationCohortCommand>? result = await this._createDissertationCohortCommandValidator.TestValidateAsync(command);

        Assert.IsFalse(result.IsValid);
        Assert.That(result.Errors.Any(e => e.ErrorMessage == "Start Date should be an earlier date than the End Date"));
    }

    [Test]
    public async Task AllConditions_Valid_ShouldPassValidation()
    {
        // Arrange
        var currentYear = DateTime.UtcNow.Year;
        var academicYearStartDate = new DateTime(currentYear, 9, 10);
        var academicYearEndDate = new DateTime(currentYear + 1, 8, 30);
        var cohortStartDate = new DateTime(currentYear, 9, 20); // September of the current year
        var cohortEndDate = new DateTime(currentYear + 1, 1, 20); // January of the next year
        var supervisionChoiceDeadline = new DateTime(currentYear, 10, 15); // A date between the start and end dates
        var academicYear = Dissertation.Domain.Entities.AcademicYear.Create(academicYearStartDate, academicYearEndDate);

        // Mock the methods to return valid conditions
        this._unitOfWork
            .Setup(x => x.DissertationCohortRepository.AnyAsync(It.IsAny<Expression<Func<Dissertation.Domain.Entities.DissertationCohort, bool>>>()))
            .ReturnsAsync(false);

        this._unitOfWork
            .Setup(x => x.AcademicYearRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Dissertation.Domain.Entities.AcademicYear, bool>>>(), It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.AcademicYear>, IOrderedQueryable<Dissertation.Domain.Entities.AcademicYear>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.AcademicYear, object>>[]>()))
            .ReturnsAsync(academicYear);

        this._unitOfWork
            .Setup(x => x.DissertationCohortRepository.GetAllAsync(
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.DissertationCohort, bool>>>(),
                It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.DissertationCohort>,
                    IOrderedQueryable<Dissertation.Domain.Entities.DissertationCohort>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.DissertationCohort, object>>[]>()))
            .ReturnsAsync(new List<Dissertation.Domain.Entities.DissertationCohort>());

        var command = new CreateDissertationCohortCommand(cohortStartDate, cohortEndDate, supervisionChoiceDeadline, 2);
        // Act
        TestValidationResult<CreateDissertationCohortCommand>? result = await this._createDissertationCohortCommandValidator.TestValidateAsync(command);
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsValid, Is.True); // The validation should pass
            Assert.That(result.Errors, Is.Empty); // There should be no validation errors
        });
    }

    [Test]
    public async Task SupervisionChoiceDeadline_ShouldBeBetweenStartAndEndDates()
    {
        this._unitOfWork
            .Setup(x => x.DissertationCohortRepository.AnyAsync(It.IsAny<Expression<Func<Dissertation.Domain.Entities.DissertationCohort, bool>>>()))
            .ReturnsAsync(false);

        this._unitOfWork
            .Setup(x => x.AcademicYearRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Dissertation.Domain.Entities.AcademicYear, bool>>>(), It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.AcademicYear>, IOrderedQueryable<Dissertation.Domain.Entities.AcademicYear>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.AcademicYear, object>>[]>()))
            .ReturnsAsync((Dissertation.Domain.Entities.AcademicYear)null!);

        this._unitOfWork
            .Setup(x => x.DissertationCohortRepository.GetAllAsync(
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.DissertationCohort, bool>>>(),
                It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.DissertationCohort>,
                    IOrderedQueryable<Dissertation.Domain.Entities.DissertationCohort>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.DissertationCohort, object>>[]>()))
            .ReturnsAsync(new List<Dissertation.Domain.Entities.DissertationCohort>());

        var command = new CreateDissertationCohortCommand(DateTime.UtcNow, DateTime.UtcNow.AddYears(1), DateTime.UtcNow.AddYears(2), 2);

        TestValidationResult<CreateDissertationCohortCommand>? result = await this._createDissertationCohortCommandValidator.TestValidateAsync(command);

        Assert.IsFalse(result.IsValid);
        Assert.That(result.Errors.Any(e => e.ErrorMessage == "Supervision Choice Deadline must be between the Start Date and End Date"));
    }

    [Test]
    public async Task SupervisionChoiceDeadline_ShouldNotBeEmpty()
    {
        this._unitOfWork
            .Setup(x => x.DissertationCohortRepository.AnyAsync(It.IsAny<Expression<Func<Dissertation.Domain.Entities.DissertationCohort, bool>>>()))
            .ReturnsAsync(false);

        this._unitOfWork
            .Setup(x => x.AcademicYearRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Dissertation.Domain.Entities.AcademicYear, bool>>>(), It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.AcademicYear>, IOrderedQueryable<Dissertation.Domain.Entities.AcademicYear>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.AcademicYear, object>>[]>()))
            .ReturnsAsync((Dissertation.Domain.Entities.AcademicYear)null!);

        this._unitOfWork
            .Setup(x => x.DissertationCohortRepository.GetAllAsync(
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.DissertationCohort, bool>>>(),
                It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.DissertationCohort>,
                    IOrderedQueryable<Dissertation.Domain.Entities.DissertationCohort>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.DissertationCohort, object>>[]>()))
            .ReturnsAsync(new List<Dissertation.Domain.Entities.DissertationCohort>());

        var command = new CreateDissertationCohortCommand(DateTime.UtcNow, DateTime.UtcNow.AddYears(1), DateTime.MinValue, 2);

        TestValidationResult<CreateDissertationCohortCommand>? result = await this._createDissertationCohortCommandValidator.TestValidateAsync(command);

        Assert.IsFalse(result.IsValid);
        Assert.That(result.Errors.Any(e => e.ErrorMessage == "Supervision Deadline is required"));
    }
}