using FluentValidation.TestHelper;
using Shared.DTO;
using UserManagement_API.Data.Models.Validators;

namespace UnitTests.UserManagementAPI.Validators;

public class EditSupervisorRequestDtoValidatorTests
{
    private EditSupervisorRequestDtoValidator _validator = null!;

    [SetUp]
    public void Setup() => this._validator = new EditSupervisorRequestDtoValidator();

    [Test]
    public void UserId_ShouldHaveError_WhenEmpty()
    {
        var model = new EditSupervisorRequestDto { UserId = string.Empty };
        TestValidationResult<EditSupervisorRequestDto>? result = this._validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(dto => dto.UserId);
    }

    // Similar tests for UserId being null, FirstName, LastName, StaffId, and DepartmentId

    [Test]
    public void FirstName_ShouldNotHaveError_WhenValid()
    {
        var model = new EditSupervisorRequestDto { FirstName = "John" };
        TestValidationResult<EditSupervisorRequestDto>? result = this._validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(dto => dto.FirstName);
    }

    // Similar tests for FirstName, LastName, StaffId with valid inputs and max length

    [Test]
    public void FirstName_ShouldHaveError_WhenContainsWhiteSpace()
    {
        var model = new EditSupervisorRequestDto { FirstName = "John Doe" };
        TestValidationResult<EditSupervisorRequestDto>? result = this._validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(dto => dto.FirstName);
    }

    // Similar tests for LastName, StaffId containing whitespaces
}