using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;
using Shared.MessageBus;
using Shared.Settings;
using UnitTests.UserManagementAPI.Mocks;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;
using UserManagement_API.Data.Models.Dto;
using UserManagement_API.Service.IService;

namespace UnitTests.UserManagementAPI.UserService;

public class EditUserTest
{
    private Mock<IHttpContextAccessor> _httpContextAccessor = null!;
    private Mock<FakeUserManager> _userManager = null!;
    private Mock<IMapper> _mapper = null!;
    private Mock<IUnitOfWork> _mockUnitOfWork = null!;
    private Mock<IAppLogger<UserManagement_API.Service.UserService>> _logger = null!;
    private Mock<IOptions<ServiceBusSettings>> _serviceBusSettings = null!;
    private Mock<IMessageBus> _messageBus = null!;
    private Mock<IDissertationApiService> _dissertationApi = null!;
    private ApplicationUser? _applicationUser = new();
    private ServiceBusSettings _serviceBusSettingsValue = new();
    private UserManagement_API.Service.UserService _userService = null!;



    [SetUp]
    public void Setup()
    {
        this._httpContextAccessor = new Mock<IHttpContextAccessor>();
        this._userManager = new Mock<FakeUserManager>();
        this._mapper = new Mock<IMapper>();
        this._logger = new Mock<IAppLogger<UserManagement_API.Service.UserService>>();
        this._mockUnitOfWork = new Mock<IUnitOfWork>();
        this._messageBus = new Mock<IMessageBus>();
        this._serviceBusSettings = new Mock<IOptions<ServiceBusSettings>>();
        this._dissertationApi = new Mock<IDissertationApiService>();
        this._serviceBusSettings.Setup(settings => settings.Value).Returns(this._serviceBusSettingsValue);
        this._userService = new UserManagement_API.Service.UserService(this._mockUnitOfWork.Object, this._logger.Object, this._mapper.Object,
            this._userManager.Object, this._messageBus.Object, this._serviceBusSettings.Object, this._dissertationApi.Object, this._httpContextAccessor.Object);

        #region TestData
        this._applicationUser = TestData.User;
        this._applicationUser.ProfilePicture = TestData.ProfilePicture;
        this._serviceBusSettingsValue = TestData.ServiceBusSettings;
        #endregion
    }

    [Test]
    public void EditUser_ShouldThrowUnauthorizedException_WhenLoggedInAdminEmailIsNull()
    {
        // Arrange
        var request = new EditUserRequestDto { /* ... populate request data ... */ };

        // Act & Assert
        UnauthorizedException? ex = Assert.ThrowsAsync<UnauthorizedException>(async () => await this._userService.EditUser(request, null));
        Assert.That(ex?.Message, Is.Not.Null);
    }

    [Test]
    public void EditUser_ShouldThrowBadRequestException_WhenValidationFails()
    {
        // Arrange
        var request = new EditUserRequestDto { /* ... populate request data ... */ };
        var loggedInAdminEmail = "admin@test.com";
        var validationResult = new ValidationResult(new List<ValidationFailure>
        {
            new ValidationFailure("Field", "Error message")
        });
        var validator = new Mock<IValidator<EditUserRequestDto>>();
        validator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);


        // Act & Assert
        BadRequestException? ex = Assert.ThrowsAsync<BadRequestException>(async () => await this._userService.EditUser(request, loggedInAdminEmail));
        Assert.That(ex?.Message, Is.EqualTo("Invalid Edit Request"));
    }

    [Test]
    public async Task EditUser_ShouldReturnFailure_WhenUserDoesNotExist()
    {
        // Arrange
        var request = new EditUserRequestDto
        {
            Email = this._applicationUser!.Email!,
            UserName = this._applicationUser.UserName!,
            FirstName = this._applicationUser.FirstName,
            LastName = this._applicationUser.LastName,
            UserId = this._applicationUser.Id
        };
        var loggedInAdminEmail = "admin@test.com";
        this._userManager.Setup(manager => manager.FindByIdAsync(request.UserId)).ReturnsAsync((ApplicationUser)null!);

        // Act
        ResponseDto<EditUserRequestDto> result = await this._userService.EditUser(request, loggedInAdminEmail);
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo($"User with this userid - {request.UserId} does not exist"));
        });
    }

    [Test]
    public async Task EditUser_ShouldReturnFailure_WhenUsernameAlreadyExists()
    {
        // Arrange
        var request = new EditUserRequestDto
        {
            Email = this._applicationUser!.Email!,
            UserName = "change-name",
            FirstName = this._applicationUser.FirstName,
            LastName = this._applicationUser.LastName,
            UserId = this._applicationUser.Id
        };
        var loggedInAdminEmail = "admin@test.com";
        this._userManager.Setup(manager => manager.FindByIdAsync(request.UserId)).ReturnsAsync(this._applicationUser);
        this._mockUnitOfWork.Setup(db => db.ApplicationUserRepository.DoesEmailExist(request.Email, It.IsAny<CancellationToken>())).ReturnsAsync(true); // Simulate email already exists
        this._mockUnitOfWork.Setup(db => db.ApplicationUserRepository.DoesUserNameExist(request.UserName, It.IsAny<CancellationToken>())).ReturnsAsync(true); // Simulate username already exists


        // Act
        ResponseDto<EditUserRequestDto> result = await this._userService.EditUser(request, loggedInAdminEmail);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Message, Is.EqualTo("Username already exists for another user"));
    }

    [Test]
    public async Task EditUser_ShouldReturnFailure_WhenEmailAlreadyExists()
    {
        // Arrange
        var request = new EditUserRequestDto
        {
            Email = "change-email@hallam.shu.ac.uk",
            UserName = this._applicationUser!.UserName!,
            FirstName = this._applicationUser.FirstName,
            LastName = this._applicationUser.LastName,
            UserId = this._applicationUser.Id
        };
        var loggedInAdminEmail = "admin@test.com";
        this._userManager.Setup(manager => manager.FindByIdAsync(request.UserId)).ReturnsAsync(this._applicationUser);
        this._mockUnitOfWork.Setup(db => db.ApplicationUserRepository.DoesEmailExist(request.Email, It.IsAny<CancellationToken>())).ReturnsAsync(true); // Simulate email already exists


        // Act
        ResponseDto<EditUserRequestDto> result = await this._userService.EditUser(request, loggedInAdminEmail);
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Email  already exists for another user"));
        });
    }

    [Test]
    public async Task EditUser_ShouldReturnSuccess_WhenUpdateIsSuccessful()
    {
        // Arrange
        var request = new EditUserRequestDto
        {
            Email = this._applicationUser!.Email!,
            UserName = this._applicationUser.UserName!,
            FirstName = this._applicationUser.FirstName,
            LastName = this._applicationUser.LastName,
            UserId = this._applicationUser.Id
        };
        var loggedInAdminEmail = "admin@test.com";
        this._userManager.Setup(manager => manager.FindByIdAsync(request.UserId)).ReturnsAsync(this._applicationUser);
        this._userManager.Setup(manager => manager.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
        this._mockUnitOfWork.Setup(db => db.ApplicationUserRepository.DoesEmailExist(request.Email, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        this._mockUnitOfWork.Setup(db => db.ApplicationUserRepository.DoesUserNameExist(request.UserName, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        ResponseDto<EditUserRequestDto> result = await this._userService.EditUser(request, loggedInAdminEmail);
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo(SuccessMessages.DefaultSuccess));
            Assert.That(result.Result, Is.EqualTo(request));
        });
    }
}