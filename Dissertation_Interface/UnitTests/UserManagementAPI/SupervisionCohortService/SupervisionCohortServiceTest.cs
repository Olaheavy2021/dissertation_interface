using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Helpers;
using UnitTests.UserManagementAPI.Mocks;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;
using UserManagement_API.Service.IService;

namespace UnitTests.UserManagementAPI.SupervisionCohortService;

public class SupervisionCohortServiceTest
{
    private Mock<IUnitOfWork> _mockUnitOfWork = null!;
    private Mock<ILogger<UserManagement_API.Service.SupervisionCohortService>> _mockLogger = null!;
    private Mock<FakeUserManager> _mockUserManager = null!;
    private Mock<IMapper> _mockMapper = null!;
    private Mock<IDissertationApiService> _mockDissertationApiService = null!;
    private UserManagement_API.Service.SupervisionCohortService _service = null!;

    [SetUp]
    public void Setup()
    {
        this._mockUnitOfWork = new Mock<IUnitOfWork>();
        this._mockLogger = new Mock<ILogger<UserManagement_API.Service.SupervisionCohortService>>();
        this._mockUserManager = new Mock<FakeUserManager>();
        this._mockMapper = new Mock<IMapper>();
        this._mockDissertationApiService = new Mock<IDissertationApiService>();

        this._service = new UserManagement_API.Service.SupervisionCohortService(this._mockUnitOfWork.Object, this._mockLogger.Object, this._mockUserManager.Object, this._mockMapper.Object, this._mockDissertationApiService.Object);
    }

    [Test]
    public async Task GetSupervisionCohort_ValidId_ReturnsSupervisionCohort()
    {
        // Arrange
        const int cohortId = 1;
        var department = new GetDepartment() { Name = "Computing", Id = 4};
        var supervisionCohort = SupervisionCohort.Create(
            "supervisorId",
            5,
            4
        );
        supervisionCohort.Supervisor = TestData.User;
        var departments = new List<GetDepartment>
        {
            department
        };

        this._mockUnitOfWork.Setup(u => u.SupervisionCohortRepository.GetAsync(It.IsAny<Expression<Func<SupervisionCohort, bool>>>(), null, x => x.Supervisor))
            .ReturnsAsync(supervisionCohort);

        this._mockDissertationApiService.Setup(s => s.GetAllDepartments())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetDepartment>> { Result = departments, IsSuccess = true });

        this._mockMapper.Setup(m => m.Map<SupervisorListDto>(It.IsAny<ApplicationUser>()))
            .Returns(new SupervisorListDto {Department = department, });

        // Act
        ResponseDto<GetSupervisionCohort> result = await this._service.GetSupervisionCohort(cohortId);
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Result, Is.Not.Null);
        });
    }

    [Test]
    public void GetSupervisionCohort_NonexistentCohort_ThrowsNotFoundException()
    {
        // Arrange
        const int invalidCohortId = 99;
        this._mockUnitOfWork.Setup(u => u.SupervisionCohortRepository.GetAsync(It.IsAny<Expression<Func<SupervisionCohort, bool>>>(),null, null))
            .ReturnsAsync((SupervisionCohort)null!);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await this._service.GetSupervisionCohort(invalidCohortId));
    }

    [Test]
    public void GetSupervisionCohort_NoDepartmentData_ThrowsNotFoundException()
    {
        // Arrange
        const int cohortId = 1;
        var supervisionCohort = SupervisionCohort.Create(
            "supervisorId",
            5,
            4
        );

        this._mockUnitOfWork.Setup(u => u.SupervisionCohortRepository.GetAsync(It.IsAny<Expression<Func<SupervisionCohort, bool>>>(), null, x => x.Supervisor))
            .ReturnsAsync(supervisionCohort);

        this._mockDissertationApiService.Setup(s => s.GetAllDepartments())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetDepartment>> { Result = null, IsSuccess = false });

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await this._service.GetSupervisionCohort(cohortId));
    }

    [Test]
    public async Task GetSupervisionCohorts_ValidParameters_ReturnsPaginatedList()
    {
        // Arrange
        var parameters = new SupervisionCohortListParameters { /* Set parameters */ };
        var supervisionCohorts = new PagedList<SupervisionCohort> { /* Populate with test data */ };
        var departments = new List<GetDepartment> { /* Set department data */ };

        this._mockUnitOfWork.Setup(u => u.SupervisionCohortRepository.GetPaginatedListOfSupervisionCohort(parameters))
            .Returns(supervisionCohorts);

        this._mockDissertationApiService.Setup(s => s.GetAllDepartments())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetDepartment>> { Result = departments, IsSuccess = true });


        // Act
        ResponseDto<PaginatedSupervisionCohortListDto> result = await this._service.GetSupervisionCohorts(parameters);
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Result!.Data, Has.Count.EqualTo(supervisionCohorts.Count));
        });
    }
}