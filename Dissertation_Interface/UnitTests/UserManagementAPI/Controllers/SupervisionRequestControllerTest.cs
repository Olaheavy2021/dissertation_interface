using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.DTO;
using UserManagement_API.Controllers;
using UserManagement_API.Service.IService;

namespace UnitTests.UserManagementAPI.Controllers;

public class SupervisionRequestControllerTest
{
    private Mock<ISupervisionRequestService> _mockSupervisionRequestService = null!;
    private SupervisionRequestController _controller = null!;

    [SetUp]
    public void Setup()
    {
        this._mockSupervisionRequestService = new Mock<ISupervisionRequestService>();
        this._controller = new SupervisionRequestController(this._mockSupervisionRequestService.Object);
    }
    [Test]
    public async Task GetSupervisionRequests_ReturnsOkResult()
    {
        // Arrange
        var parameters = new SupervisionRequestPaginationParameters(); // Setup parameters
        var responseDto = new ResponseDto<PaginatedSupervisionRequestListDto>();

        this._mockSupervisionRequestService.Setup(s => s.GetPaginatedListOfSupervisionRequest(parameters))
                                     .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.GetSupervisionRequests(parameters);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task GetSupervisionRequestsForAStudent_ReturnsOkResult()
    {
        // Arrange
        var parameters = new SupervisionRequestPaginationParameters(); // Setup parameters
        var responseDto = new ResponseDto<PaginatedSupervisionRequestListDto>();

        this._mockSupervisionRequestService.Setup(s => s.GetPaginatedListOfSupervisionRequestForAStudent(parameters))
                                     .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.GetSupervisionRequestsForAStudent(parameters);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task GetSupervisionRequestsForASupervisor_ReturnsOkResult()
    {
        // Arrange
        var parameters = new SupervisionRequestPaginationParameters(); // Setup parameters
        var responseDto = new ResponseDto<PaginatedSupervisionRequestListDto>();

        this._mockSupervisionRequestService.Setup(s => s.GetPaginatedListOfSupervisionRequestForASupervisor(parameters))
                                     .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.GetSupervisionRequestsForASupervisor(parameters);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task CancelSupervisionRequest_ReturnsOkResult()
    {
        // Arrange
        var model = new ActionSupervisionRequest(); // Setup model
        var responseDto = new ResponseDto<string>();

        this._mockSupervisionRequestService.Setup(s => s.CancelSupervisionRequest(model, It.IsAny<CancellationToken>()))
                                     .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.CancelSupervisionRequest(model);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task RejectSupervisionRequest_ReturnsOkResult()
    {
        // Arrange
        var model = new ActionSupervisionRequest(); // Setup model
        var responseDto = new ResponseDto<string>();

        this._mockSupervisionRequestService.Setup(s => s.RejectSupervisionRequest(model, It.IsAny<CancellationToken>()))
                                     .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.RejectSupervisionRequest(model);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
    }

    [Test]
    public async Task AcceptSupervisionRequest_ReturnsOkResult()
    {
        // Arrange
        var model = new ActionSupervisionRequest(); // Setup model
        var responseDto = new ResponseDto<string>();

        this._mockSupervisionRequestService.Setup(s => s.AcceptSupervisionRequest(model, It.IsAny<CancellationToken>()))
                                     .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.AcceptSupervisionRequest(model);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task CreateSupervisionRequest_ReturnsOkResult()
    {
        // Arrange
        var model = new CreateSupervisionRequest(); // Setup model
        var responseDto = new ResponseDto<string>();

        this._mockSupervisionRequestService.Setup(s => s.CreateSupervisionRequest(model, It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.CreateSupervisionRequest(model);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

}