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
using UserManagement_API.Service.IService;

namespace UnitTests.UserManagementAPI.UserService;

public class EditSupervisorTest
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
        this._userService = new UserManagement_API.Service.UserService(this._mockUnitOfWork.Object,this._logger.Object,this._mapper.Object,
            this._userManager.Object, this._messageBus.Object, this._serviceBusSettings.Object, this._dissertationApi.Object, this._httpContextAccessor.Object);

        #region TestData
        this._applicationUser = TestData.User;
        this._applicationUser.ProfilePicture = TestData.ProfilePicture;
        this._serviceBusSettingsValue = TestData.ServiceBusSettings;
        #endregion
    }

    [Test]
    public void EditSupervisor_ShouldThrowUnauthorizedException_WhenLoggedInUserIsNull()
    {
        // Arrange
        var request = new EditSupervisorRequestDto { /* ... populate request data ... */ };

        // Act & Assert
        UnauthorizedException? ex = Assert.ThrowsAsync<UnauthorizedException>(async () => await this._userService.EditSupervisor(request, null));
        Assert.That(ex!.Message, Is.Not.Null);
    }

    [Test]
    public void EditSupervisor_ShouldThrowBadRequestException_WhenValidationFails()
    {
        // Arrange
        var request = new EditSupervisorRequestDto { /* ... populate request data ... */ };
        var loggedInUser = "user@test.com";
        var validationResult = new ValidationResult(new List<ValidationFailure>
        {
            new ValidationFailure("Field", "Error message")
        });
        var validator = new Mock<IValidator<EditSupervisorRequestDto>>();
        validator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);

        // Act & Assert
        BadRequestException? ex = Assert.ThrowsAsync<BadRequestException>(async () => await this._userService.EditSupervisor(request, loggedInUser));
        Assert.That(ex?.Message, Is.EqualTo("Invalid Edit Request"));
    }

    [Test]
    public async Task EditSupervisor_ShouldReturnFailure_WhenSupervisorDoesNotExist()
    {
        // Arrange
        var request = new EditSupervisorRequestDto
        {
            DepartmentId = 1,
            StaffId = "staff",
            FirstName = this._applicationUser!.FirstName!,
            UserId = this._applicationUser!.Id,
            LastName = this._applicationUser.LastName
        };
        var loggedInUser = "user@test.com";
        this._userManager.Setup(manager => manager.FindByIdAsync(request.UserId)).ReturnsAsync((ApplicationUser)null!);

        // Act
        ResponseDto<UserDto> result = await this._userService.EditSupervisor(request, loggedInUser);
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo($"Supervisor with this userid - {request.UserId} does not exist"));
        });
    }

    [Test]
    public async Task EditSupervisor_ShouldReturnSuccess_WhenUpdateIsSuccessful()
    {
        // Arrange
        var request = new EditSupervisorRequestDto
        {
            StaffId = this._applicationUser!.UserName!,
            FirstName = this._applicationUser.FirstName,
            LastName = this._applicationUser.LastName,
            UserId = this._applicationUser.Id,
            DepartmentId = 1
        };
        var loggedInUser = "user@test.com";
        var user = new ApplicationUser { /* ... populate user data ... */ };
        var mappedUserDto = new UserDto { /* ... populate mapped user dto data ... */ };
        this._userManager.Setup(manager => manager.FindByIdAsync(request.UserId)).ReturnsAsync(user);
        this._userManager.Setup(manager => manager.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
        this._mockUnitOfWork.Setup(db => db.ApplicationUserRepository.DoesUserNameExist(request.StaffId, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        this._mapper.Setup(mapper => mapper.Map<UserDto>(It.IsAny<ApplicationUser>())).Returns(mappedUserDto);

        // Act
        ResponseDto<UserDto> result = await this._userService.EditSupervisor(request, loggedInUser);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(result.Message, Is.EqualTo(SuccessMessages.DefaultSuccess));
            Assert.That(result.Result, Is.EqualTo(mappedUserDto));
        });
    }
}