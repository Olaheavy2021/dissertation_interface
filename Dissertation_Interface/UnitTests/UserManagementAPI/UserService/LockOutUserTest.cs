using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using Shared.Constants;
using Shared.DTO;
using Shared.Logging;
using Shared.MessageBus;
using Shared.Settings;
using UnitTests.UserManagementAPI.Mocks;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;
using UserManagement_API.Service.IService;

namespace UnitTests.UserManagementAPI.UserService;

public class LockOutUserTest
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
    public async Task LockOutUser_ShouldReturnFailure_WhenUserIsNotFound()
    {
        // Arrange
        var email = "nonExistingUser@test.com";
        this._userManager.Setup(manager => manager.FindByEmailAsync(email))
            .ReturnsAsync((ApplicationUser)null!);

        // Act
        ResponseDto<bool> result = await this._userService.LockOutUser(email, null);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Result, Is.False);
            Assert.That(result.Message, Is.EqualTo("An error occurred whilst locking out the user"));
        });
    }

    [Test]
    public async Task LockOutUser_ShouldReturnFailure_WhenUserIsSystemDefault()
    {
        // Arrange
        var user = new ApplicationUser { UserName = SystemDefault.DefaultSuperAdmin1 };
        this._userManager.Setup(manager => manager.FindByEmailAsync(user.Email!)).ReturnsAsync(user);

        // Act
        ResponseDto<bool> result = await this._userService.LockOutUser(user.Email!, user.Email);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Result, Is.False);
            Assert.That(result.Message, Is.EqualTo("Lock out failed. This user is a system default user"));
        });
    }

    [Test]
    public async Task LockOutUser_ShouldReturnSuccess_WhenLockOutIsSuccessful()
    {
        // Arrange
        var email = "existingUser@test.com";
        var user = new ApplicationUser { Email = email };
        this._userManager.Setup(manager => manager.FindByEmailAsync(email)).ReturnsAsync(user);
        this._userManager.Setup(manager => manager.SetLockoutEndDateAsync(user, SystemDefault.LockOutEndDate)).ReturnsAsync(IdentityResult.Success);
        this._userManager.Setup(manager => manager.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        ResponseDto<bool> result = await this._userService.LockOutUser(email, null);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Result, Is.True);
            Assert.That(result.Message, Is.EqualTo("User locked out successfully"));
        });
    }
}