using System.Linq.Expressions;
using FluentValidation.TestHelper;
using Moq;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;
using UserManagement_API.Data.Models.Dto;
using UserManagement_API.Data.Models.Validators;

namespace UnitTests.UserManagementAPI.Validators;

[TestFixture]
public class RegisterRequestDtoValidatorTests
{
    private RegisterRequestDtoValidator _validator = null!;
    private Mock<IUnitOfWork> _mockUnitOfWork = null!;

    [SetUp]
    public void Setup()
    {
        this._mockUnitOfWork = new Mock<IUnitOfWork>();
        this._validator = new RegisterRequestDtoValidator(this._mockUnitOfWork.Object);
    }

    [Test]
    public async Task UserDetails_ShouldBeUnique()
    {
        var model = new RegistrationRequestDto { UserName = "ExistingUser", Email = "existing@domain.com" };
        this._mockUnitOfWork.Setup(u => u.ApplicationUserRepository.AnyAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
            .ReturnsAsync(true);

        TestValidationResult<RegistrationRequestDto>? result = await this._validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor("Custom");
    }

    [Test]
    public async Task UserDetails_ShouldBeValid_IfUnique()
    {
        var model = new RegistrationRequestDto { UserName = "NewUser", Email = "new@domain.com" };
        this._mockUnitOfWork.Setup(u => u.ApplicationUserRepository.AnyAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
            .ReturnsAsync(false);

        TestValidationResult<RegistrationRequestDto>? result = await this._validator.TestValidateAsync(model);
        result.ShouldNotHaveValidationErrorFor("Custom");
    }
}