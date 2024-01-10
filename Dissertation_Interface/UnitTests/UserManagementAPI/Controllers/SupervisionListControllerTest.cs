using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.DTO;
using UserManagement_API.Controllers;
using UserManagement_API.Service.IService;

namespace UnitTests.UserManagementAPI.Controllers;

public class SupervisionListControllerTest
{
    private Mock<ISupervisionListService> _mockSupervisionListService = null!;
    private SupervisionListController _controller = null!;

    [SetUp]
    public void Setup()
    {
        this._mockSupervisionListService = new Mock<ISupervisionListService>();
        this._controller = new SupervisionListController(this._mockSupervisionListService.Object);
    }

    [Test]
    public async Task GetSupervisionList_ReturnsOkResult()
    {
        // Arrange
        var parameters = new SupervisionListPaginationParameters(); // Setup parameters
        var responseDto = new ResponseDto<PaginatedSupervisionListDto>();

        this._mockSupervisionListService.Setup(s => s.GetPaginatedListOfSupervisionRequest(parameters))
            .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.GetSupervisionList(parameters);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task GetSupervisionListsForAStudent_ReturnsOkResult()
    {
        // Arrange
        var parameters = new SupervisionListPaginationParameters(); // Setup parameters
        var responseDto = new ResponseDto<PaginatedSupervisionListDto>();

        this._mockSupervisionListService.Setup(s => s.GetPaginatedListOfSupervisionListForAStudent(parameters))
            .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.GetSupervisionListsForAStudent(parameters);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task GetSupervisionListsForASupervisor_ReturnsOkResult()
    {
        // Arrange
        var parameters = new SupervisionListPaginationParameters(); // Setup parameters
        var responseDto = new ResponseDto<PaginatedSupervisionListDto>();

        this._mockSupervisionListService.Setup(s => s.GetPaginatedListOfSupervisionListForASupervisor(parameters))
            .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.GetSupervisionListsForASupervisor(parameters);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }
}