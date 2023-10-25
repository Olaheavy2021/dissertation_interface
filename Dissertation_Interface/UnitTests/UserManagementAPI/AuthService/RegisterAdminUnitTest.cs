using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Helpers;
using Shared.Logging;
using Shared.MessageBus;
using Shared.Settings;
using UnitTests.UserManagementAPI.Mocks;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;
using UserManagement_API.Data.Models.Dto;

namespace UnitTests.UserManagementAPI.AuthService;

public class RegisterAdminUnitTest
{
    private Mock<FakeUserManager>? _userManagerMock;
    private Mock<FakeSignInManager>? _signInManagerMock;
    private Mock<IUnitOfWork>? _unitOfWork;
    private Mock<IOptions<ApplicationUrlSettings>>? _applicationUrlSettings;
    private Mock<IMessageBus>? _messageBus;
    private Mock<IOptions<JwtSettings>>? _jwtSettings;
    private Mock<IAppLogger<UserManagement_API.Service.AuthService>>? _logger;
    private Mock<IMapper>? _mapperMock;
    private Mock<ITokenManager>? _tokenManager;
    private Mock<IOptions<ServiceBusSettings>>? _serviceBusSettings;
    private AdminRegistrationRequestDto _adminRegistrationRequestDto = new();
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
        this._tokenManager = new Mock<ITokenManager>();
        #endregion

        #region TestData
        this._adminRegistrationRequestDto = TestData.AdminRegistrationRequestDto;
        this._applicationUrlSettingsValue = TestData.ApplicationUrlSettings;
        this._serviceBusSettingsValue = TestData.ServiceBusSettings;
        #endregion
    }

    [TearDown]
    public void TearDown() => this._userManagerMock?.Object.Dispose();

    [Test]
    public async Task RegisterAdmin_Successfully()
    {
        #region Arrange
        this._unitOfWork
            ?.Setup(unitofwork =>
                unitofwork.ApplicationUserRepository.AnyAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
            .ReturnsAsync(false);

        this._userManagerMock?.Setup(userManager => userManager.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .Returns(Task.FromResult(IdentityResult.Success));

        this._userManagerMock?.Setup(userManager => userManager.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .Returns(Task.FromResult(IdentityResult.Success));

        this._messageBus?.Setup(bus =>
            bus.PublishMessage(It.IsAny<PublishEmailDto>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();

        this._applicationUrlSettings?.Setup(settings =>
            settings.Value).Returns(this._applicationUrlSettingsValue);

        this._serviceBusSettings?.Setup(settings => settings.Value).Returns(this._serviceBusSettingsValue);
        #endregion

        #region Act
        var authService = new UserManagement_API.Service.AuthService(
            this._unitOfWork?.Object!, this._applicationUrlSettings?.Object!, this._messageBus?.Object!, this._jwtSettings!.Object,
            this._signInManagerMock?.Object!, this._userManagerMock?.Object!, this._logger?.Object!, this._mapperMock!.Object,
            this._serviceBusSettings?.Object!,this._tokenManager?.Object!
        );
        ResponseDto<string> result = await authService.RegisterAdmin(this._adminRegistrationRequestDto,this._adminRegistrationRequestDto.Email);
        #endregion

        #region Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
        });

        #endregion

    }

    [Test]
    public void RegisterAdmin_UserExists()
    {
        #region Arrange
        this._unitOfWork
            ?.Setup(unitofwork =>
                unitofwork.ApplicationUserRepository.AnyAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
            .Returns(Task.FromResult(true));
        #endregion

        #region Act
        var authService = new UserManagement_API.Service.AuthService(
            this._unitOfWork?.Object!, this._applicationUrlSettings?.Object!, this._messageBus?.Object!, this._jwtSettings!.Object,
            this._signInManagerMock?.Object!, this._userManagerMock?.Object!, this._logger?.Object!, this._mapperMock!.Object,
            this._serviceBusSettings?.Object!, this._tokenManager?.Object!
        );
        #endregion

        #region Assert
        BadRequestException? ex = Assert.ThrowsAsync<BadRequestException>(async () => await authService.RegisterAdmin(this._adminRegistrationRequestDto, this._adminRegistrationRequestDto.Email));
        StringAssert.Contains("Invalid Registration Request", ex?.Message);
        #endregion
    }
}