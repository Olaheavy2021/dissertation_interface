using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.DTO;
using Shared.Exceptions;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;
using UserManagement_API.Service.IService;

namespace UnitTests.UserManagementAPI.SupervisionCohortService;

public class UpdateSupervisionSlotTest
{
    private Mock<IUnitOfWork> _mockUnitOfWork = null!;
    private Mock<ILogger<UserManagement_API.Service.SupervisionCohortService>> _mockLogger = null!;
    private Mock<FakeUserManager> _userManagerMock = null!; // Assuming ApplicationUser is the user entity
    private Mock<IMapper> _mockMapper = null!;
    private Mock<IDissertationApiService> _mockDissertationApiService = null!;
    private UserManagement_API.Service.SupervisionCohortService _service = null!;

    [SetUp]
    public void Setup()
    {
        this._mockUnitOfWork = new Mock<IUnitOfWork>();
        this._mockLogger = new Mock<ILogger<UserManagement_API.Service.SupervisionCohortService>>();
        this._userManagerMock = new Mock<FakeUserManager>();
        this._mockMapper = new Mock<IMapper>();
        this._mockDissertationApiService = new Mock<IDissertationApiService>();

        this._service = new UserManagement_API.Service.SupervisionCohortService(this._mockUnitOfWork.Object, this._mockLogger.Object, this._userManagerMock.Object, this._mockMapper.Object, this._mockDissertationApiService.Object);
    }

    [Test]
    public void UpdateSupervisionSlot_NonexistentCohort_ThrowsNotFoundException()
    {
        // Arrange
        var request = new UpdateSupervisionCohortRequest { SupervisionCohortId = 99, SupervisionSlots = 10 };
        this._mockUnitOfWork.Setup(u => u.SupervisionCohortRepository.GetAsync(It.IsAny<Expression<Func<SupervisionCohort, bool>>>(), null, null))
            .ReturnsAsync((SupervisionCohort)null!);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await this._service.UpdateSupervisionSlot(request, CancellationToken.None));
    }
}