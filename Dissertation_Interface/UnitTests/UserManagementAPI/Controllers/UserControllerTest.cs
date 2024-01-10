using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.DTO;
using UserManagement_API.Controllers;
using UserManagement_API.Data.Models.Dto;
using UserManagement_API.Service.IService;

namespace UnitTests.UserManagementAPI.Controllers;

public class UserControllerTest
{
    private Mock<IAuthService> _mockAuthService = null!;
    private Mock<IUserService> _mockUserService = null!;
    private UserController _controller = null!;

    private void SetupHttpContext(string email, string userId)
    {
        var mockHttpContext = new Mock<HttpContext>();
        var items = new Dictionary<object, object>
        {
            { "Email", email },
            { "UserId", userId }
        };

        mockHttpContext.Setup(c => c.Items).Returns(items!);

        this._controller.ControllerContext = new ControllerContext
        {
            HttpContext = mockHttpContext.Object
        };
    }

    [SetUp]
    public void Setup()
    {
        this._mockAuthService = new Mock<IAuthService>();
        this._mockUserService = new Mock<IUserService>();
        this._controller = new UserController(this._mockAuthService.Object, this._mockUserService.Object);
        SetupHttpContext("test@example.com", "12345");
    }

    [Test]
    public async Task RegisterAdmin_ReturnsOkResult()
    {
        // Arrange
        var model = new AdminRegistrationRequestDto(); // Setup model
        var responseDto = new ResponseDto<string>();

        this._mockAuthService.Setup(s => s.RegisterAdmin(model, It.IsAny<string>()))
            .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.RegisterAdmin(model);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    // Test for ResendConfirmationEmail
    [Test]
    public async Task ResendConfirmationEmail_ReturnsOkResult()
    {
        // Arrange
        var model = new EmailRequestDto(); // Setup model
        var responseDto = new ResponseDto<string>();

        this._mockAuthService.Setup(s => s.ResendConfirmationEmail(model, It.IsAny<string>()))
                        .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.ResendConfirmationEmail(model);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    // Test for GetUser (access-token)
    [Test]
    public async Task GetUser_ReturnsOkResult()
    {
        // Arrange
        var responseDto = new ResponseDto<GetUserDto>();

        this._mockUserService.Setup(s => s.GetUser())
                        .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.GetUser();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    // Test for GetUser by ID
    [Test]
    public async Task GetUserById_ReturnsOkResult()
    {
        // Arrange
        var id = "someId";
        var responseDto = new ResponseDto<GetUserDto>();

        this._mockUserService.Setup(s => s.GetUser(id))
                        .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.GetUser(id);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    // Test for GetUserByEmail
    [Test]
    public async Task GetUserByEmail_ReturnsOkResult()
    {
        // Arrange
        const string email = "test@example.com";
        var responseDto = new ResponseDto<GetUserDto>();

        this._mockUserService.Setup(s => s.GetUserByEmail(email))
                        .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.GetUserByEmail(email);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    // Test for GetUserByUserName
    [Test]
    public async Task GetUserByUserName_ReturnsOkResult()
    {
        // Arrange
        var username = "testuser";
        var responseDto = new ResponseDto<GetUserDto>();

        this._mockUserService.Setup(s => s.GetUserByUserName(username))
                        .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.GetUserByUserName(username);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    // Test for LockOutUser
    [Test]
    public async Task LockOutUser_ReturnsOkResult()
    {
        // Arrange
        var model = new EmailRequestDto { Email = "test@example.com" };
        var responseDto = new ResponseDto<bool>();

        this._mockUserService.Setup(s => s.LockOutUser(model.Email, It.IsAny<string>()))
                        .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.LockOutUser(model);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    // Test for UnlockUser
    [Test]
    public async Task UnlockUser_ReturnsOkResult()
    {
        // Arrange
        var model = new EmailRequestDto { Email = "test@example.com" };
        var responseDto = new ResponseDto<bool>();

        this._mockUserService.Setup(s => s.UnlockUser(model.Email, It.IsAny<string>()))
                        .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.UnlockUser(model);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    // Test for GetAdminUsers
    [Test]
    public void GetAdminUsers_ReturnsOkResultWithPagination()
    {
        // Arrange
        var paginationParameters = new UserPaginationParameters();
        var responseDto = new ResponseDto<PaginatedUserListDto>
        {
            Result = new PaginatedUserListDto() // Populate with necessary data
        };

        this._mockUserService.Setup(s => s.GetPaginatedAdminUsers(paginationParameters))
                        .Returns(responseDto);

        // Act
        ActionResult result = this._controller.GetAdminUsers(paginationParameters);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
    }

    // Test for GetStudents
    [Test]
    public async Task GetStudents_ReturnsOkResultWithPagination()
    {
        // Arrange
        var paginationParameters = new DissertationStudentPaginationParameters();
        var responseDto = new ResponseDto<PaginatedStudentListDto>
        {
            Result = new PaginatedStudentListDto() // Populate with necessary data
        };

        this._mockUserService.Setup(s => s.GetPaginatedStudents(paginationParameters))
                        .ReturnsAsync(responseDto);

        // Act
        ActionResult result = await this._controller.GetStudents(paginationParameters);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.Multiple(() =>
        {
            Assert.That(okResult, Is.Not.Null);
        });
    }

    // Test for GetSupervisors
    [Test]
    public async Task GetSupervisors_ReturnsOkResultWithPagination()
    {
        // Arrange
        var paginationParameters = new SupervisorPaginationParameters();
        var responseDto = new ResponseDto<PaginatedSupervisorListDto>
        {
            Result = new PaginatedSupervisorListDto() // Populate with necessary data
        };

        this._mockUserService.Setup(s => s.GetPaginatedSupervisors(paginationParameters))
                        .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.GetSupervisors(paginationParameters);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.Multiple(() =>
        {
            Assert.That(okResult, Is.Not.Null);
        });
    }

    // Test for EditUser
    [Test]
    public async Task EditUser_ReturnsOkResult()
    {
        // Arrange
        var model = new EditUserRequestDto(); // Setup model
        var responseDto = new ResponseDto<EditUserRequestDto>();

        this._mockUserService.Setup(s => s.EditUser(model, It.IsAny<string>()))
                        .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.EditUser(model);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    // Test for RegisterSupervisor
    [Test]
    public async Task RegisterSupervisor_ReturnsOkResult()
    {
        // Arrange
        var model = new StudentOrSupervisorRegistrationDto(); // Setup model
        var responseDto = new ResponseDto<string>();

        this._mockAuthService.Setup(s => s.RegisterSupervisor(model))
                        .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.RegisterSupervisor(model);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
    }

    // Test for RegisterStudent
    [Test]
    public async Task RegisterStudent_ReturnsOkResult()
    {
        // Arrange
        var model = new StudentOrSupervisorRegistrationDto(); // Setup model
        var responseDto = new ResponseDto<string>();

        this._mockAuthService.Setup(s => s.RegisterStudent(model))
                        .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.RegisterStudent(model);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
    }

    // Test for EditStudent
    [Test]
    public async Task EditStudent_ReturnsOkResult()
    {
        // Arrange
        var model = new EditStudentRequestDto(); // Setup model
        var responseDto = new ResponseDto<UserDto>();

        this._mockUserService.Setup(s => s.EditStudent(model, It.IsAny<string>()))
                        .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.EditStudent(model);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    // Test for EditSupervisor
    [Test]
    public async Task EditSupervisor_ReturnsOkResult()
    {
        // Arrange
        var model = new EditSupervisorRequestDto(); // Setup model
        var responseDto = new ResponseDto<UserDto>();

        this._mockUserService.Setup(s => s.EditSupervisor(model, It.IsAny<string>()))
                        .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.EditSupervisor(model);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    // Test for AssignSupervisorRoleToAdmin
    [Test]
    public async Task AssignSupervisorRoleToAdmin_ReturnsOkResult()
    {
        // Arrange
        var model = new AssignSupervisorRoleRequestDto(); // Setup model
        var responseDto = new ResponseDto<UserDto>();

        this._mockAuthService.Setup(s => s.AssignSupervisorRoleToAdmin(model, It.IsAny<string>()))
                        .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.AssignSupervisorRoleToAdmin(model);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
    }

    // Test for ChangeAdminRole
    [Test]
    public async Task ChangeAdminRole_ReturnsOkResult()
    {
        // Arrange
        var model = new EmailRequestDto(); // Setup model
        var responseDto = new ResponseDto<UserDto>();

        this._mockAuthService.Setup(s => s.ChangeAdminRole(model, It.IsAny<string>()))
                        .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.ChangeAdminRole(model);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
    }

    // Test for AssignAdminRoleToSupervisor
    [Test]
    public async Task AssignAdminRoleToSupervisor_ReturnsOkResult()
    {
        // Arrange
        var model = new AssignAdminRoleRequestDto(); // Setup model
        var responseDto = new ResponseDto<UserDto>();

        this._mockAuthService.Setup(s => s.AssignAdminRoleToSupervisor(model, It.IsAny<string>()))
                        .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.AssignAdminRoleToSupervisor(model);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }
}