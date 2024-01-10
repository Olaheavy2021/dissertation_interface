using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.DTO;
using UserManagement_API.Controllers;
using UserManagement_API.Service.IService;

namespace UnitTests.UserManagementAPI.Controllers;

public class SupervisionCohortControllerTest
{
    private Mock<ISupervisionCohortService> _mockSupervisionCohortService = null!;
    private SupervisionCohortController _controller = null!;

    [SetUp]
    public void Setup()
    {
        this._mockSupervisionCohortService = new Mock<ISupervisionCohortService>();
        this._controller = new SupervisionCohortController(this._mockSupervisionCohortService.Object);
    }

    [Test]
    public async Task CreateSupervisionCohort_ReturnsOkResult_WithValidModel()
    {
        // Arrange
        var createSupervisionCohortListRequest = new CreateSupervisionCohortListRequest { /* set properties */ };
        var responseDto = new ResponseDto<string> { /* set properties */ };

        this._mockSupervisionCohortService.Setup(service => service.CreateSupervisionCohort(createSupervisionCohortListRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.CreateSupervisionCohort(createSupervisionCohortListRequest);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task GetSupervisionCohorts_ReturnsOkResult()
    {
        // Arrange
        var parameters = new SupervisionCohortListParameters(); // Set up parameters as needed
        var responseDto = new ResponseDto<PaginatedSupervisionCohortListDto>();

        this._mockSupervisionCohortService.Setup(service => service.GetSupervisionCohorts(parameters))
            .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.GetSupervisionCohorts(parameters);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task GetSupervisionCohort_ReturnsOkResult()
    {
        // Arrange
        long id = 1; // Example ID
        var responseDto = new ResponseDto<GetSupervisionCohort>();

        this._mockSupervisionCohortService.Setup(service => service.GetSupervisionCohort(id))
            .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.GetSupervisionCohort(id);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task GetInActiveSupervisorsForCohort_ReturnsOkResult()
    {
        // Arrange
        var parameters = new SupervisionCohortListParameters(); // Set up parameters as needed
        var responseDto = new ResponseDto<PaginatedUserListDto>();

        this._mockSupervisionCohortService.Setup(service => service.GetInActiveSupervisorsForCohort(parameters))
            .Returns(responseDto); // Note: This method is not async

        // Act
        IActionResult result = await this._controller.GetInActiveSupervisorsForCohort(parameters);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task UpdateSupervisionSlot_ReturnsOkResult()
    {
        // Arrange
        var request = new UpdateSupervisionCohortRequest(); // Set up request as needed
        var responseDto = new ResponseDto<string>();

        this._mockSupervisionCohortService.Setup(service => service.UpdateSupervisionSlot(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.UpdateSupervisionSlot(request);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task DeleteSupervisionCohort_ReturnsOkResult()
    {
        // Arrange
        long supervisionCohortId = 1; // Example ID
        var responseDto = new ResponseDto<string>();

        this._mockSupervisionCohortService.Setup(service => service.DeleteSupervisionCohort(supervisionCohortId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.DeleteSupervisionCohort(supervisionCohortId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task GetSupervisionCohortMetrics_ReturnsOkResult()
    {
        // Arrange
        long cohortId = 1; // Example ID
        var responseDto = new ResponseDto<SupervisionCohortMetricsDto>();

        this._mockSupervisionCohortService.Setup(service => service.GetSupervisionCohortMetrics(cohortId))
            .ReturnsAsync(responseDto);

        // Act
        IActionResult result = await this._controller.GetSupervisionCohortMetrics(cohortId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }




}