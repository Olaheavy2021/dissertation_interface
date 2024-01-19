using FluentValidation.TestHelper;
using UserManagement_API.Data.Models.Dto;
using UserManagement_API.Data.Models.Validators;

namespace UnitTests.UserManagementAPI.Validators;

[TestFixture]
public class EditUserRequestDtoValidatorTests
{
    private EditUserRequestDtoValidator _validator = null!;

    [SetUp]
    public void Setup() => this._validator = new EditUserRequestDtoValidator();

    [Test]
    public void UserId_ShouldHaveError_WhenEmpty()
    {
        var model = new EditUserRequestDto { UserId = string.Empty };
        TestValidationResult<EditUserRequestDto>? result = this._validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(dto => dto.UserId);
    }

    // Similar tests for UserId being null, FirstName, LastName, UserName, and Email

    [Test]
    public void FirstName_ShouldNotHaveError_WhenValid()
    {
        var model = new EditUserRequestDto { FirstName = "John" };
        TestValidationResult<EditUserRequestDto>? result = this._validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(dto => dto.FirstName);
    }

    // Similar tests for FirstName, LastName, UserName with valid inputs and max length

    [Test]
    public void Email_ShouldHaveError_WhenInvalidFormat()
    {
        var model = new EditUserRequestDto { Email = "john@example.com" };
        TestValidationResult<EditUserRequestDto>? result = this._validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(dto => dto.Email);
    }

    [Test]
    public void Email_ShouldNotHaveError_WhenValidHallamEmail()
    {
        var model = new EditUserRequestDto { Email = "john@student.shu.ac.uk" };
        TestValidationResult<EditUserRequestDto>? result = this._validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(dto => dto.Email);
    }
}