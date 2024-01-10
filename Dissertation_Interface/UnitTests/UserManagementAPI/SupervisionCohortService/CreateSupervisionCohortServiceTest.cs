using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.DTO;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;
using UserManagement_API.Service.IService;

namespace UnitTests.UserManagementAPI.SupervisionCohortService;

public class CreateSupervisionCohortServiceTest
{
    private Mock<IUnitOfWork> _mockUnitOfWork = null!;
    private Mock<ILogger<UserManagement_API.Service.SupervisionCohortService>> _mockLogger = null!;
    private Mock<IMapper> _mockMapper = null!;
    private Mock<IDissertationApiService> _mockDissertationApiService = null!;
    private UserManagement_API.Service.SupervisionCohortService _service = null!;
    private Mock<FakeUserManager> _userManagerMock = null!;

    [SetUp]
    public void Setup()
    {
        this._mockUnitOfWork = new Mock<IUnitOfWork>();
        this._mockLogger = new Mock<ILogger<UserManagement_API.Service.SupervisionCohortService>>();
        this._mockMapper = new Mock<IMapper>();
        this._mockDissertationApiService = new Mock<IDissertationApiService>();
        this._userManagerMock = new Mock<FakeUserManager>();
        this._service = new UserManagement_API.Service.SupervisionCohortService(this._mockUnitOfWork.Object, this._mockLogger.Object, this._userManagerMock.Object, this._mockMapper.Object, this._mockDissertationApiService.Object);
    }

    [Test]
    public async Task CreateSupervisionCohort_WithInvalidDissertationCohort_ReturnsFailure()
    {
        // Arrange
        const int invalidDissertationCohortId = 99;
        var request = new CreateSupervisionCohortListRequest { DissertationCohortId = invalidDissertationCohortId };
        this._mockDissertationApiService.Setup(s => s.GetActiveDissertationCohort())
            .ReturnsAsync(new ResponseDto<GetDissertationCohort> { Result = new GetDissertationCohort { Id = 1 }, IsSuccess = true });

        // Act
        ResponseDto<string> result = await this._service.CreateSupervisionCohort(request, CancellationToken.None);
        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo($"Invalid Dissertation cohort - {invalidDissertationCohortId}"));
        });
    }
}