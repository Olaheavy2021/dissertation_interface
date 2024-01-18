using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
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
using UserManagement_API.Service.IService;

namespace UnitTests.UserManagementAPI.UserService;

public class GetUserWithUserIdTest
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
    private UserDto _userDto = new();
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
        this._userService = new UserManagement_API.Service.UserService(this._mockUnitOfWork.Object,this._logger.Object,this._mapper.Object,
            this._userManager.Object, this._messageBus.Object, this._serviceBusSettings.Object, this._dissertationApi.Object, this._httpContextAccessor.Object);

        #region TestData
        this._applicationUser = TestData.User;
        this._applicationUser.ProfilePicture = TestData.ProfilePicture;
        this._userDto = TestData.UserDtoResponse;
        #endregion
    }

    [Test]
    public async Task GetUser_ShouldReturnFailure_WhenUserIsNotFound()
    {
        // Arrange
        var userId = "nonExistingUserId";
        this._mockUnitOfWork.Setup(repo => repo.ApplicationUserRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
                null,
                x => x.ProfilePicture!))
            .ReturnsAsync((ApplicationUser)null!);

        // Act
        ResponseDto<GetUserDto> result = await this._userService.GetUser(userId);
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("No user found"));
            Assert.That(result.Result, Is.Null);
        });
    }

    [Test]
    public async Task GetUser_ShouldReturnValidUserDto_WhenUserIsFound()
    {
        // Arrange
        var userId = "existingUserId";
        var roles = new List<string> { "Role1", "Role2" };

        this._httpContextAccessor.Setup(x => x.HttpContext!.Items["UserId"]).Returns(userId);
        this._mockUnitOfWork.Setup(repo => repo.ApplicationUserRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
                null,
                x => x.ProfilePicture!))
            .ReturnsAsync(this._applicationUser);
        this._userManager.Setup(manager => manager.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(roles);
        this._mapper.Setup(mapper => mapper.Map<ApplicationUser, UserDto>(It.IsAny<ApplicationUser>())).Returns(this._userDto);
        this._mapper.Setup(mapper => mapper.Map<ProfilePicture, GetProfilePicture>(It.IsAny<ProfilePicture>())).Returns(new GetProfilePicture());

        // Act
        ResponseDto<GetUserDto> result = await this._userService.GetUser(userId);
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo(SuccessMessages.DefaultSuccess));
        });
        Assert.That(result.Result, Is.Not.Null);
        Assert.That(result.Result?.User, Is.EqualTo(this._userDto)); // Assumes your UserDto has a proper equality check
    }
}