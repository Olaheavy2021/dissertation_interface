using AutoMapper;
using Microsoft.Extensions.Options;
using Moq;
using Shared.DTO;
using Shared.Helpers;
using Shared.Logging;
using Shared.MessageBus;
using Shared.Settings;
using UnitTests.UserManagementAPI.Mocks;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;
using UserManagement_API.Data.Models.Dto;

namespace UnitTests.UserManagementAPI.AuthService;

public class ResendConfirmationEmailUnitTest
{
    private Mock<FakeUserManager>? _userManagerMock;
    private Mock<FakeSignInManager>? _signInManagerMock;
    private Mock<IUnitOfWork>? _unitOfWork;
    private Mock<IOptions<ApplicationUrlSettings>>? _applicationUrlSettings;
    private Mock<IMessageBus>? _messageBus;
    private Mock<IOptions<JwtSettings>>? _jwtSettings;
    private Mock<IAppLogger<UserManagement_API.Service.AuthService>>? _logger;
    private Mock<IMapper>? _mapperMock;
    private Mock<IOptions<ServiceBusSettings>>? _serviceBusSettings;
    private ApplicationUser? _applicationUser = new();
    private EmailRequestDto _emailRequestDto = new();
    private ApplicationUrlSettings _applicationUrlSettingsValue = new();
    private ServiceBusSettings _serviceBusSettingsValue = new();

    [SetUp]
    public void Setup()
    {
        #region MockedDependencies
        this._unitOfWork = new Mock<IUnitOfWork>();
        this._applicationUrlSettings = new Mock<IOptions<ApplicationUrlSettings>>();
        this._messageBus = new Mock<IMessageBus>();
        this._jwtSettings = new Mock<IOptions<JwtSettings>>();
        this._logger = new Mock<IAppLogger<UserManagement_API.Service.AuthService>>();
        this._mapperMock = new Mock<IMapper>();
        this._serviceBusSettings = new Mock<IOptions<ServiceBusSettings>>();
        this._signInManagerMock = new Mock<FakeSignInManager>();
        this._userManagerMock = new Mock<FakeUserManager>();
        #endregion

        #region TestData
        this._applicationUser = TestData.User;
        this._emailRequestDto = TestData.EmailRequestDto;
        this._applicationUrlSettingsValue = TestData.ApplicationUrlSettings;
        this._serviceBusSettingsValue = TestData.ServiceBusSettings;
        #endregion
    }

    [TearDown]
    public void TearDown() => this._userManagerMock?.Object.Dispose();

    [Test]
    public async Task ResendConfirmationEmail_Failed()
    {
        #region Arrange
        this._applicationUser!.EmailConfirmed = true;
        this._userManagerMock?.Setup(userManager =>
            userManager.FindByEmailAsync(It.IsAny<string>())
        ).Returns(Task.FromResult(this._applicationUser)!);

        this._serviceBusSettings?.Setup(settings => settings.Value).Returns(this._serviceBusSettingsValue);

        this._messageBus?.Setup(bus =>
            bus.PublishAuditLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();
        #endregion

        #region Act
        var authService = new UserManagement_API.Service.AuthService(
            this._unitOfWork?.Object!, this._applicationUrlSettings?.Object!, this._messageBus?.Object!, this._jwtSettings!.Object,
            this._signInManagerMock?.Object!, this._userManagerMock?.Object!, this._logger?.Object!, this._mapperMock!.Object,
            this._serviceBusSettings?.Object!
        );

        ResponseDto<string> result = await authService.ResendConfirmationEmail(this._emailRequestDto, this._emailRequestDto.Email);
        #endregion

        #region Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.Not.True);
        });
        #endregion

    }

    [Test]
    public async Task ResendConfirmationEmail_Successful()
    {
        #region Arrange
        this._applicationUser!.EmailConfirmed = false;
        this._userManagerMock?.Setup(userManager =>
            userManager.FindByEmailAsync(It.IsAny<string>())
        ).Returns(Task.FromResult(this._applicationUser)!);

        this._userManagerMock?.Setup(userManager =>
            userManager.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())
        ).Returns(Task.FromResult("token"));

        this._applicationUrlSettings?.Setup(settings =>
            settings.Value).Returns(this._applicationUrlSettingsValue);

        this._serviceBusSettings?.Setup(settings => settings.Value).Returns(this._serviceBusSettingsValue);

        this._messageBus?.Setup(bus =>
            bus.PublishMessage(It.IsAny<PublishEmailDto>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();

        this._messageBus?.Setup(bus =>
            bus.PublishAuditLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();
        #endregion

        #region Act
        var authService = new UserManagement_API.Service.AuthService(
            this._unitOfWork?.Object!, this._applicationUrlSettings?.Object!, this._messageBus?.Object!, this._jwtSettings!.Object,
            this._signInManagerMock?.Object!, this._userManagerMock?.Object!, this._logger?.Object!, this._mapperMock!.Object,
            this._serviceBusSettings?.Object!
        );

        ResponseDto<string> result = await authService.ResendConfirmationEmail(this._emailRequestDto, this._emailRequestDto.Email);
        #endregion

        #region Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
        });
        #endregion

    }
}