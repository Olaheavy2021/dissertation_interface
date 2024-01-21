using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.DTO;
using UserManagement_API.Controllers;
using UserManagement_API.Data.Models.Dto;
using UserManagement_API.Service.IService;

namespace UnitTests.UserManagementAPI.Controllers;

public class AuthControllerTest
{
    private Mock<IAuthService>? _mockAuthService;
    private Mock<IProfilePictureService>? _mockProfilePictureService;

    [SetUp]
    public void Setup()
    {
        #region MockedDependencies

        this._mockAuthService = new Mock<IAuthService>();
        this._mockProfilePictureService = new Mock<IProfilePictureService>();
        #endregion
    }

    [Test]
    public async Task Login_ReturnsOkResult_WhenCredentialsAreValid()
    {
        //Arrange
        var loginDto = new LoginRequestDto { Password = "Password10$", Email = "email@shu.ac.uk" };
        var authResponseDto = new ResponseDto<AuthResponseDto> { IsSuccess = true, };
        this._mockAuthService?.Setup(service => service.Login(It.IsAny<LoginRequestDto>())).ReturnsAsync(authResponseDto);

        var controller = new AuthController(this._mockAuthService?.Object!, this._mockProfilePictureService?.Object!);

        // Act
        IActionResult result = await controller.Login(loginDto);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var returnValue = okResult?.Value as ResponseDto<AuthResponseDto>;
        Assert.That(returnValue, Is.Not.Null);
        Assert.That(returnValue!.IsSuccess, Is.True);
    }

    [Test]
    public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInValid()
    {
        //Arrange
        var loginDto = new LoginRequestDto { Password = "Password10$", Email = "email@shu.ac.uk" };
        var authResponseDto = new ResponseDto<AuthResponseDto> { IsSuccess = false, };
        this._mockAuthService?.Setup(service => service.Login(It.IsAny<LoginRequestDto>())).ReturnsAsync(authResponseDto);

        var controller = new AuthController(this._mockAuthService?.Object!, this._mockProfilePictureService?.Object!);

        // Act
        IActionResult result = await controller.Login(loginDto);

        // Assert
        Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
    }

    [Test]
    public async Task InitiateResetPassword_ReturnsOkResult_WithValidModel()
    {
        var responseDto = new ResponseDto<string> { IsSuccess = true };
        this._mockAuthService?.Setup(service => service.InitiatePasswordReset(It.IsAny<InitiatePasswordResetDto>())).ReturnsAsync(responseDto);
        var initiateResetDto = new InitiatePasswordResetDto
        {
            Email = "email@shu.ac.uk",
        };

        var controller = new AuthController(this._mockAuthService!.Object, this._mockProfilePictureService!.Object);

        // Act
        IActionResult result = await controller.InitiateResetPassword(initiateResetDto);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task ConfirmResetPassword_ReturnsOkResult_WithValidModel()
    {
        var responseDto = new ResponseDto<string> { IsSuccess = true };
        this._mockAuthService?.Setup(service => service.ConfirmPasswordReset(It.IsAny<ConfirmPasswordResetDto>())).ReturnsAsync(responseDto);
        var confirmPasswordResetDto = new ConfirmPasswordResetDto
        {
            Password = "Password",
            UserName = "kakakka"
        };

        this._mockAuthService?.Setup(service => service.ConfirmPasswordReset(confirmPasswordResetDto))
            .ReturnsAsync(responseDto);

        var controller = new AuthController(this._mockAuthService!.Object, this._mockProfilePictureService!.Object);

        // Act
        IActionResult result = await controller.ConfirmResetPassword(confirmPasswordResetDto);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task RefreshToken_ReturnsOkResult_WithValidModel()
    {
        var responseDto = new ResponseDto<RefreshTokenDto> { IsSuccess = true };
        this._mockAuthService?.Setup(service => service.GetRefreshToken(It.IsAny<RefreshTokenDto>())).ReturnsAsync(responseDto);
        var refreshTokenDto = new RefreshTokenDto()
        {
            RefreshToken = "abcde",
            AccessToken = "abcde"
        };

        this._mockAuthService?.Setup(service => service.GetRefreshToken(refreshTokenDto))
            .ReturnsAsync(responseDto);

        var controller = new AuthController(this._mockAuthService!.Object, this._mockProfilePictureService!.Object);

        // Act
        IActionResult result = await controller.RefreshToken(refreshTokenDto);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task UpdateProfile_ReturnsOkResult_WithValidModel()
    {
        var responseDto = new ResponseDto<string> { IsSuccess = true };
        this._mockProfilePictureService?.Setup(service => service.UploadProfilePicture(It.IsAny<ProfilePictureUploadRequestDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(responseDto);
        var profilePictureUploadRequestDto = new ProfilePictureUploadRequestDto
        {
            FirstName = "abcde",
            LastName = "abcde"
        };

        var controller = new AuthController(this._mockAuthService!.Object, this._mockProfilePictureService!.Object);

        // Act
        IActionResult result = await controller.UpdateProfile(profilePictureUploadRequestDto);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task ConfirmEmail_ReturnsOKResult_WithValidModel()
    {
        var responseDto = new ResponseDto<ConfirmEmailResponseDto> { IsSuccess = true };
        this._mockAuthService?.Setup(service => service.ConfirmEmail(It.IsAny<ConfirmEmailRequestDto>())).ReturnsAsync(responseDto);
        var requestDto = new ConfirmEmailRequestDto() { UserName = "c234563", Token = "hgdsgsdh" };

        var controller = new AuthController(this._mockAuthService!.Object, this._mockProfilePictureService!.Object);

        // Act
        IActionResult result = await controller.ConfirmEmail(requestDto);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }
}