using AutoMapper;
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
using UserManagement_API.Data.Models.Dto;

namespace UnitTests.UserManagementAPI.AuthService;

[TestFixture]
public class UserRoleServiceTests
{
    private Mock<FakeUserManager>? _userManagerMock;
    private Mock<FakeSignInManager>? _signInManagerMock;
    private Mock<IUnitOfWork>? _unitOfWork;
    private Mock<IOptions<ApplicationUrlSettings>>? _applicationUrlSettings;
    private Mock<IMessageBus>? _messageBus;
    private Mock<IOptions<JwtSettings>>? _jwtSettings;
    private Mock<IAppLogger<UserManagement_API.Service.AuthService>>? _logger;
    private UserManagement_API.Service.AuthService _authService = null!;
    private Mock<IMapper>? _mapperMock;
    private Mock<IOptions<ServiceBusSettings>>? _serviceBusSettings;
    private ApplicationUser? _applicationUser = new();


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
        this._authService = new UserManagement_API.Service.AuthService(
            this._unitOfWork?.Object!, this._applicationUrlSettings?.Object!, this._messageBus?.Object!, this._jwtSettings!.Object,
            this._signInManagerMock?.Object!, this._userManagerMock?.Object!, this._logger?.Object!, this._mapperMock!.Object,
            this._serviceBusSettings?.Object!
        );
        #endregion

        #region TestData

        this._applicationUser = TestData.User;

        #endregion
    }

    [Test]
    public async Task AssignSupervisorRoleToAdmin_UserIsNotAdminOrSuperadmin_ReturnsFailure()
    {
        // Arrange
        var requestDto = new AssignSupervisorRoleRequestDto { /* ... populate with test data ... */ };
        var user = new ApplicationUser { /* ... populate with test data ... */ };
        var roles = new List<string> { /* ... populate with roles that do not include admin or superadmin ... */ };

        this._userManagerMock!.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        this._userManagerMock.Setup(um => um.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(roles);

        // Act
        ResponseDto<UserDto> result = await this._authService.AssignSupervisorRoleToAdmin(requestDto, requestDto.Email);
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("User is not an admin or superadmin"));
        });
    }

    [Test]
    public async Task AssignSupervisorRoleToAdmin_SuccessfullyAssignsRole_ReturnsSuccess()
    {
        // Arrange
        var requestDto = new AssignSupervisorRoleRequestDto { /* ... populate with test data ... */ };
        ApplicationUser? user = this._applicationUser;
        user!.EmailConfirmed = true;
        var roles = new List<string> { Roles.RoleAdmin };
        var mappedUser = new UserDto { /* ... populate with test data ... */ };
        IdentityResult identityResult = IdentityResult.Success;

        this._userManagerMock?.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        this._userManagerMock?.Setup(um => um.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(roles);
        this._userManagerMock?.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(identityResult);
        this._mapperMock?.Setup(m => m.Map<UserDto>(It.IsAny<ApplicationUser>())).Returns(mappedUser);

        // Act
        ResponseDto<UserDto> result = await this._authService.AssignSupervisorRoleToAdmin(requestDto, requestDto.Email);
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo(SuccessMessages.DefaultSuccess));
            Assert.That(result.Result, Is.EqualTo(mappedUser));
        });
    }

    [Test]
    public async Task AssignAdminRoleToSupervisor_UserAlreadyAdminOrSuperadmin_ReturnsFailure()
    {
        // Arrange
        var request = new AssignAdminRoleRequestDto { /* ... populate with test data ... */ };
        var user = new ApplicationUser { /* ... populate with test data ... */ };
        var roles = new List<string> { Roles.RoleAdmin };

        this._userManagerMock?.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        this._userManagerMock?.Setup(um => um.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(roles);

        // Act
        ResponseDto<UserDto> result = await this._authService.AssignAdminRoleToSupervisor(request, request.Email);
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("User is already an admin or superadmin"));
        });
    }

    [Test]
    public async Task AssignAdminRoleToSupervisor_UserNotSupervisor_ReturnsFailure()
    {
        // Arrange
        var request = new AssignAdminRoleRequestDto { /* ... populate with test data ... */ };
        var user = new ApplicationUser { /* ... populate with test data ... */ };
        var roles = new List<string> { /* ... populate with roles that do not include supervisor ... */ };

        this._userManagerMock?.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        this._userManagerMock?.Setup(um => um.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(roles);

        // Act
        ResponseDto<UserDto> result = await this._authService.AssignAdminRoleToSupervisor(request, request.Email);
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("User is not assigned a supervisor role"));
        });
    }

    [Test]
    public async Task AssignAdminRoleToSupervisor_SuccessfullyAssignsRole_ReturnsSuccess()
    {
        // Arrange
        var request = new AssignAdminRoleRequestDto { /* ... populate with test data ... */ };
        var user = new ApplicationUser { /* ... populate with test data ... */ };
        var roles = new List<string> { Roles.RoleSupervisor };
        var mappedUser = new UserDto { /* ... populate with test data ... */ };
        IdentityResult identityResult = IdentityResult.Success;

        this._userManagerMock?.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        this._userManagerMock?.Setup(um => um.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(roles);
        this._userManagerMock?.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(identityResult);
        this._mapperMock?.Setup(m => m.Map<UserDto>(It.IsAny<ApplicationUser>())).Returns(mappedUser);

        // Act
        ResponseDto<UserDto> result = await this._authService.AssignAdminRoleToSupervisor(request, request.Email);
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo(SuccessMessages.DefaultSuccess));
            Assert.That(result.Result, Is.EqualTo(mappedUser));
        });
    }

    [Test]
    public async Task ChangeAdminRole_FromAdminToSuperAdmin_Success()
    {
        // Arrange
        var request = new EmailRequestDto { /* ... populate with test data ... */ };
        var user = new ApplicationUser { /* ... populate with test data ... */ };
        var roles = new List<string> { Roles.RoleAdmin };
        var mappedUser = new UserDto { /* ... populate with test data ... */ };
        IdentityResult identityResult = IdentityResult.Success;

        this._userManagerMock?.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        this._userManagerMock?.Setup(um => um.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(roles);
        this._userManagerMock?.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), Roles.RoleSuperAdmin)).ReturnsAsync(identityResult);
        this._mapperMock?.Setup(m => m.Map<UserDto>(It.IsAny<ApplicationUser>())).Returns(mappedUser);

        // Act
        ResponseDto<UserDto> result = await this._authService.ChangeAdminRole(request, request.Email);
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo(SuccessMessages.DefaultSuccess));
            Assert.That(result.Result, Is.EqualTo(mappedUser));
        });
    }

    [Test]
    public async Task ChangeAdminRole_FromSuperAdminToAdmin_Success()
    {
        // Arrange
        var request = new EmailRequestDto { /* ... populate with test data ... */ };
        var user = new ApplicationUser { /* ... populate with test data ... */ };
        var roles = new List<string> { Roles.RoleSuperAdmin };
        var mappedUser = new UserDto { /* ... populate with test data ... */ };
        IdentityResult identityResult = IdentityResult.Success;

        this._userManagerMock?.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        this._userManagerMock?.Setup(um => um.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(roles);
        this._userManagerMock?.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), Roles.RoleAdmin)).ReturnsAsync(identityResult);
        this._mapperMock?.Setup(m => m.Map<UserDto>(It.IsAny<ApplicationUser>())).Returns(mappedUser);

        // Act
        ResponseDto<UserDto> result = await this._authService.ChangeAdminRole(request, request.Email);
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo(SuccessMessages.DefaultSuccess));
            Assert.That(result.Result, Is.EqualTo(mappedUser));
        });
    }

    [Test]
    public async Task ChangeAdminRole_UserNotAdminOrSuperAdmin_ReturnsFailure()
    {
        // Arrange
        var request = new EmailRequestDto { /* ... populate with test data ... */ };
        var user = new ApplicationUser { /* ... populate with test data ... */ };
        var roles = new List<string> { /* ... populate with roles that do not include admin or superadmin ... */ };

        this._userManagerMock?.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        this._userManagerMock?.Setup(um => um.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(roles);

        // Act
        ResponseDto<UserDto> result = await this._authService.ChangeAdminRole(request, request.Email);
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("This user is not an admin"));
        });
    }
}