using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
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

[TestFixture]
public class LoginUnitTest
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
    private LoginRequestDto _loginRequestDto = new();
    private ApplicationUser? _applicationUser = new();
    private JwtSettings _jwtSettingsValue = new();
    private UserDto _userDto = new();


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
        this._loginRequestDto = TestData.LoginRequest;
        this._applicationUser = TestData.User;
        this._jwtSettingsValue = TestData.JwtSettings;
        this._userDto = TestData.UserDtoResponse;
        #endregion
    }

    [TearDown]
    public void TearDown() => this._userManagerMock?.Object.Dispose();

    [Test]
    public async Task Login_WithCorrectDetails()
    {
        //arrange
        #region AutoMapperSetup
        this._mapperMock?.Setup(mapper =>
            mapper.Map<UserDto>(It.IsAny<ApplicationUser>())).Returns(this._userDto);
        #endregion

        #region SignInManagerSetup
        this._signInManagerMock?.Setup(signInManager =>
            signInManager.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(),
                It.IsAny<bool>())
        ).Returns(Task.FromResult(SignInResult.Success));
        #endregion


        #region UserManagerSetup
        IList<Claim> userClaim = TestData.UserClaims;
        IList<string> role = new List<string> { "Admin" };

        this._userManagerMock?.Setup(userManager =>
            userManager.FindByEmailAsync(It.IsAny<string>())
        ).Returns(Task.FromResult(this._applicationUser));

        this._userManagerMock?.Setup(userManager =>
            userManager.GetRolesAsync(It.IsAny<ApplicationUser>())
        ).Returns(Task.FromResult(role));

        this._userManagerMock?.Setup(userManager =>
            userManager.UpdateAsync(It.IsAny<ApplicationUser>())
        ).Returns(Task.FromResult(IdentityResult.Success));

        this._userManagerMock?.Setup(userManager =>
            userManager.GetClaimsAsync(It.IsAny<ApplicationUser>())
        ).Returns(Task.FromResult(userClaim));
        #endregion

        #region JwtSettingsSetup

        this._jwtSettings?.Setup(jwtSettings =>
            jwtSettings.Value).Returns(this._jwtSettingsValue);

        #endregion


        //act
        var authService = new UserManagement_API.Service.AuthService(
            this._unitOfWork?.Object!, this._applicationUrlSettings?.Object!, this._messageBus?.Object!, this._jwtSettings!.Object,
            this._signInManagerMock?.Object!, this._userManagerMock?.Object!, this._logger?.Object!, this._mapperMock!.Object,
            this._serviceBusSettings?.Object!, this._tokenManager?.Object!
        );
        ResponseDto<AuthResponseDto> result = await authService.Login(this._loginRequestDto);

        //assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Result?.Role, Is.Not.Empty);
            Assert.That(result.Result?.AccessToken, Is.Not.Empty);
            Assert.That(result.Result?.RefreshToken, Is.Not.Empty);
            Assert.That(result.Result?.User, Is.TypeOf<UserDto>());
        });
    }

    [Test]
    public async Task Login_WithIncorrectDetails()
    {
        //arrange
        this._signInManagerMock?.Setup(signInManager =>
            signInManager.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(),
                It.IsAny<bool>())
        ).Returns(Task.FromResult(SignInResult.Failed));

        //act
        var authService = new UserManagement_API.Service.AuthService(
            this._unitOfWork?.Object!, this._applicationUrlSettings?.Object!, this._messageBus?.Object!, this._jwtSettings!.Object,
            this._signInManagerMock?.Object!, this._userManagerMock?.Object!, this._logger?.Object!, this._mapperMock!.Object,
            this._serviceBusSettings?.Object!, this._tokenManager?.Object!
        );
        ResponseDto<AuthResponseDto> result = await authService.Login(this._loginRequestDto);

        //assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.Not.True);
            Assert.That(result.Result, Is.Null);
        });
    }

    [Test]
    public async Task Login_WithUnConfirmedEmail()
    {
        //arrange
        this._signInManagerMock?.Setup(signInManager =>
            signInManager.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(),
                It.IsAny<bool>())
        ).Returns(Task.FromResult(SignInResult.NotAllowed));

        //act
        var authService = new UserManagement_API.Service.AuthService(
            this._unitOfWork?.Object!, this._applicationUrlSettings?.Object!, this._messageBus?.Object!, this._jwtSettings!.Object,
            this._signInManagerMock?.Object!, this._userManagerMock?.Object!, this._logger?.Object!, this._mapperMock!.Object,
            this._serviceBusSettings?.Object!, this._tokenManager?.Object!
        );
        ResponseDto<AuthResponseDto> result = await authService.Login(this._loginRequestDto);

        //assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.Not.True);
            Assert.That(result.Result, Is.Null);
        });
    }

    [Test]
    public async Task Login_WithLockedAccount()
    {
        //arrange
        this._signInManagerMock?.Setup(signInManager =>
            signInManager.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(),
                It.IsAny<bool>())
        ).Returns(Task.FromResult(SignInResult.LockedOut));

        //act
        var authService = new UserManagement_API.Service.AuthService(
            this._unitOfWork?.Object!, this._applicationUrlSettings?.Object!, this._messageBus?.Object!, this._jwtSettings!.Object,
            this._signInManagerMock?.Object!, this._userManagerMock?.Object!, this._logger?.Object!, this._mapperMock!.Object,
            this._serviceBusSettings?.Object!, this._tokenManager?.Object!
        );
        ResponseDto<AuthResponseDto> result = await authService.Login(this._loginRequestDto);

        //assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.Not.True);
            Assert.That(result.Result, Is.Null);
        });
    }
}