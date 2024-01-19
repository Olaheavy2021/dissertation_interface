using FluentValidation.TestHelper;
using UserManagement_API.Data.Models.Dto;
using UserManagement_API.Data.Models.Validators;

namespace UnitTests.UserManagementAPI.Validators;

[TestFixture]
public class LoginRequestDtoValidatorTests
{
    private LoginRequestDtoValidator _validator = null!;

    [SetUp]
    public void Setup() => this._validator = new LoginRequestDtoValidator();

    [Test]
    public void Password_ShouldHaveError_WhenNull()
    {
        var model = new LoginRequestDto { Password = null! };
        TestValidationResult<LoginRequestDto>? result = this._validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(dto => dto.Password);
    }

    [Test]
    public void Password_ShouldHaveError_WhenEmpty()
    {
        var model = new LoginRequestDto { Password = string.Empty };
        TestValidationResult<LoginRequestDto>? result = this._validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(dto => dto.Password);
    }

    [Test]
    public void Email_ShouldHaveError_WhenNull()
    {
        var model = new LoginRequestDto { Email = null! };
        TestValidationResult<LoginRequestDto>? result = this._validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(dto => dto.Email);
    }

    [Test]
    public void Email_ShouldHaveError_WhenEmpty()
    {
        var model = new LoginRequestDto { Email = string.Empty };
        TestValidationResult<LoginRequestDto>? result = this._validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(dto => dto.Email);
    }

    [Test]
    public void Email_ShouldHaveError_WhenInvalidFormat()
    {
        var model = new LoginRequestDto { Email = "invalidemail@" };
        TestValidationResult<LoginRequestDto>? result = this._validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(dto => dto.Email);
    }

    [Test]
    public void Email_ShouldNotHaveError_WhenValidFormat()
    {
        var model = new LoginRequestDto { Email = "john@student.shu.ac.uk" };
        TestValidationResult<LoginRequestDto>? result = this._validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(dto => dto.Email);
    }
}