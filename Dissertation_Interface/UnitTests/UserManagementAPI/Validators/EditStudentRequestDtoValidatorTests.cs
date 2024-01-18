using FluentValidation.TestHelper;
using Shared.DTO;
using UserManagement_API.Data.Models.Validators;

namespace UnitTests.UserManagementAPI.Validators;

[TestFixture]
public class EditStudentRequestDtoValidatorTests
{
    private EditStudentRequestDtoValidator _validator = null!;

    [SetUp]
    public void Setup() => this._validator = new EditStudentRequestDtoValidator();

    [Test]
    public void UserId_ShouldHaveError_WhenEmpty()
    {
        var model = new EditStudentRequestDto { UserId = string.Empty };
        TestValidationResult<EditStudentRequestDto>? result = this._validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(dto => dto.UserId);
    }

    // Similar tests for UserId being null, FirstName, LastName, StudentId

    [Test]
    public void FirstName_ShouldNotHaveError_WhenValid()
    {
        var model = new EditStudentRequestDto { FirstName = "John" };
        TestValidationResult<EditStudentRequestDto>? result = this._validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(dto => dto.FirstName);
    }

    // Similar tests for FirstName, LastName, StudentId with valid inputs and max length

    [Test]
    public void CourseId_ShouldHaveError_WhenEmpty()
    {
        var model = new EditStudentRequestDto { CourseId = 0 }; // Assuming CourseId is a numeric type
        TestValidationResult<EditStudentRequestDto>? result = this._validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(dto => dto.CourseId);
    }
}