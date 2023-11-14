using Dissertation.Application.AcademicYear.Commands.CreateAcademicYear;
using Dissertation.Application.DTO;
using Dissertation.Infrastructure.Persistence.IRepository;
using FluentValidation.TestHelper;
using Moq;
using UnitTests.DissertationAPI.Mocks;

namespace UnitTests.DissertationAPI.Application.AcademicYear.Validator;

public class CreateAcademicYearCommandValidatorTest
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CreateAcademicYearCommandValidator _createAcademicYearCommandValidator;

    public CreateAcademicYearCommandValidatorTest() =>
        this._createAcademicYearCommandValidator = new CreateAcademicYearCommandValidator(
            this._unitOfWork.Object
        );

    [Test]
    public async Task Should_Not_Have_Error_When_Command_IsValid()
    {
        this._unitOfWork
            .Setup(x => x.AcademicYearRepository.IsAcademicYearUnique(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(true);

        CreateAcademicYearRequest request = AcademicYearMocks.GetSuccessfulRequest();
        CreateAcademicYearCommand command = new(request.StartDate, request.EndDate);

        TestValidationResult<CreateAcademicYearCommand>? result = await this._createAcademicYearCommandValidator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async Task Should_Have_Error_When_AcademicYear_IsNotUnique()
    {
        this._unitOfWork
            .Setup(x => x.AcademicYearRepository.IsAcademicYearUnique(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(false);

        CreateAcademicYearRequest request = AcademicYearMocks.GetSuccessfulRequest();
        CreateAcademicYearCommand command = new(request.StartDate, request.EndDate);

        TestValidationResult<CreateAcademicYearCommand>? result = await this._createAcademicYearCommandValidator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(a => a.StartDate);
    }

    [Test]
    public async Task Should_Have_Error_When_AcademicYear_IsInvalid()
    {
        this._unitOfWork
            .Setup(x => x.AcademicYearRepository.IsAcademicYearUnique(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(true);

        CreateAcademicYearRequest request = AcademicYearMocks.GetFailedRequest();
        CreateAcademicYearCommand command = new(request.StartDate, request.EndDate);

        TestValidationResult<CreateAcademicYearCommand>? result = await this._createAcademicYearCommandValidator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(a => a.StartDate);
        result.ShouldHaveValidationErrorFor(a => a.EndDate);
    }
}