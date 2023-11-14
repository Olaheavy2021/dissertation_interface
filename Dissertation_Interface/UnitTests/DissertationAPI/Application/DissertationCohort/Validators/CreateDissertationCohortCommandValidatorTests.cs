using System.Linq.Expressions;
using Dissertation.Application.DissertationCohort.Commands.CreateDissertationCohort;
using Dissertation.Application.DTO.Request;
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
    public async Task Should_Not_Have_Error_When_Command_IsValid()
    {
        Dissertation.Domain.Entities.AcademicYear academicYear = AcademicYearMocks.GetFirstOrDefaultResponse();
        this._unitOfWork
            .Setup(x => x.AcademicYearRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Dissertation.Domain.Entities.AcademicYear, bool>>>(),  It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.AcademicYear>, IOrderedQueryable<Dissertation.Domain.Entities.AcademicYear>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.AcademicYear, object>>[]>()))
            .ReturnsAsync(academicYear);

        CreateDissertationCohortRequest request = DissertationCohortMocks.GetSuccessfulRequest(academicYear.StartDate, academicYear.EndDate);
        CreateDissertationCohortCommand command = new(request.StartDate, request.EndDate, request.SupervisionChoiceDeadline, request.AcademicYearId);

        TestValidationResult<CreateDissertationCohortCommand>? result = await this._createDissertationCohortCommandValidator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async Task Should_Have_Error_When_AcademicYear_IsNull()
    {
        Dissertation.Domain.Entities.AcademicYear academicYear = AcademicYearMocks.GetFirstOrDefaultResponse();
        this._unitOfWork
            .Setup(x => x.AcademicYearRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Dissertation.Domain.Entities.AcademicYear, bool>>>(),  It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.AcademicYear>, IOrderedQueryable<Dissertation.Domain.Entities.AcademicYear>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.AcademicYear, object>>[]>()))
            .ReturnsAsync((Dissertation.Domain.Entities.AcademicYear)null!);

        CreateDissertationCohortRequest request = DissertationCohortMocks.GetSuccessfulRequest(academicYear.StartDate, academicYear.EndDate);
        CreateDissertationCohortCommand command = new(request.StartDate, request.EndDate, request.SupervisionChoiceDeadline, request.AcademicYearId);

        TestValidationResult<CreateDissertationCohortCommand>? result = await this._createDissertationCohortCommandValidator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.StartDate);
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
        result.ShouldHaveValidationErrorFor(x => x.AcademicYearId);
    }
}