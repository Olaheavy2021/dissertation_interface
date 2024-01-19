using FluentValidation.TestHelper;
using UserManagement_API.Data.Models.Dto;
using UserManagement_API.Data.Models.Validators;

namespace UnitTests.UserManagementAPI.Validators;

public class AdminRegistrationRequestDtoValidatorTests
{
    private AdminRegistrationRequestDtoValidator _validator = null!;

    [SetUp]
    public void Setup() => this._validator = new AdminRegistrationRequestDtoValidator();

    [TestCase("Superadmin")]
    [TestCase("Admin")]
    public void ShouldNotHaveValidationErrorForValidRole(string roleName)
    {
        var model = new AdminRegistrationRequestDto { Role = roleName };
        TestValidationResult<AdminRegistrationRequestDto>? result = this._validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(request => request.Role);
    }

    [Test]
    public void ShouldHaveValidationErrorForInvalidRole()
    {
        var model = new AdminRegistrationRequestDto { Role = "InvalidRoleName" };
        TestValidationResult<AdminRegistrationRequestDto>? result = this._validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(request => request.Role);
    }
}