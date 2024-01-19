using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Notification_API.Controllers;
using Notification_API.Data;
using Notification_API.Data.Models;
using Notification_API.Data.Models.Dto;
using Notification_API.Services;
using Shared.DTO;
using Shared.Helpers;

namespace UnitTests.NotificationAPI.Controllers;

[TestFixture]
public class AuditLogControllerTests
{
    private Mock<AuditLogService> _mockAuditLogService = null!;
    private AuditLogController _controller = null!;
    private readonly DbContextOptions<NotificationDbContext> _dbOptions = null!;

    [SetUp]
    public void SetUp()
    {
        // Create a dummy DbContextOptions instance
        this._mockAuditLogService = new Mock<AuditLogService>(this._dbOptions);
        this._controller = new AuditLogController(this._mockAuditLogService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Test]
    public async Task GetAuditLogs_ReturnsOkResult_WithPagedList()
    {
        // Arrange
        var parameters = new AuditLogPaginationParameters { /* ... populate with test data ... */ };
        var auditLogs = new List<AuditLog>();
        var pagedAuditLogs = new PagedList<AuditLog>(
            auditLogs,
            1, 10, 2);
        this._mockAuditLogService.Setup(service => service.GetListOfAuditLogs(parameters))
            .ReturnsAsync(new ResponseDto<PagedList<AuditLog>> { IsSuccess = true, Result = pagedAuditLogs });

        // Act
        IActionResult result = await this._controller.GetAuditLogs(parameters);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult?.Value, Is.Not.Null);
        Assert.That(okResult?.Value, Is.InstanceOf<ResponseDto<PagedList<AuditLog>>>());
        var response = okResult?.Value as ResponseDto<PagedList<AuditLog>>;
        Assert.That(response?.Result, Is.EqualTo(pagedAuditLogs));
        this._mockAuditLogService.Verify(service => service.GetListOfAuditLogs(parameters), Times.Once());
    }

    [Test]
    public async Task GetAuditLog_ReturnsOkResult_WithAuditLog()
    {
        // Arrange
        long id = 1;
        var auditLog = new AuditLog { /* ... populate with test data ... */ };
        this._mockAuditLogService.Setup(service => service.GetAuditLog(id))
            .ReturnsAsync(new ResponseDto<AuditLog> { IsSuccess = true, Result = auditLog });

        // Act
        IActionResult result = await this._controller.GetAuditLog(id);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult?.Value, Is.Not.Null);
        Assert.That(okResult?.Value, Is.InstanceOf<ResponseDto<AuditLog>>());
        var response = okResult?.Value as ResponseDto<AuditLog>;
        Assert.That(response?.Result, Is.EqualTo(auditLog));
        this._mockAuditLogService.Verify(service => service.GetAuditLog(id), Times.Once());
    }
}