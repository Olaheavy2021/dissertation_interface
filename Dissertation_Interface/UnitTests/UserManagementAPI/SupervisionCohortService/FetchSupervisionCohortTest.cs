using System.Linq.Expressions;
using AutoMapper;
using Dissertation.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Helpers;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;
using UserManagement_API.Service.IService;

namespace UnitTests.UserManagementAPI.SupervisionCohortService;

[TestFixture]
public class FetchSupervisionCohortTest
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
    public void GetSupervisionCohort_NotFound_ThrowsNotFoundException()
    {
        // Arrange
        long id = 1;
        this._mockUnitOfWork.Setup(um => um.SupervisionCohortRepository.GetAsync(It.IsAny<Expression<Func<SupervisionCohort, bool>>>(), null, x => x.Supervisor))
            .ReturnsAsync((SupervisionCohort)null!);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await this._service.GetSupervisionCohort(id));
    }

    [Test]
    public async Task GetSupervisionCohort_Success()
    {
        // Arrange
        long id = 1;
        var supervisionCohort = SupervisionCohort.Create(
            "supervisor",
            2,
            20243
        );
        supervisionCohort.Id = id;
        var departmentList = new List<GetDepartment> { /* ... populate with test data ... */ };
        var supervisorListDto = new SupervisorListDto { /* ... populate with test data ... */ };

        this._mockUnitOfWork.Setup(um => um.SupervisionCohortRepository.GetAsync(It.IsAny<Expression<Func<SupervisionCohort, bool>>>(), null, x => x.Supervisor))
            .ReturnsAsync(supervisionCohort);
        this._mockDissertationApiService.Setup(service => service.GetAllDepartments())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetDepartment>> { IsSuccess = true, Result = departmentList });
        this._mockMapper.Setup(m => m.Map<SupervisorListDto>(It.IsAny<Supervisor>())).Returns(supervisorListDto);

        // Act
        ResponseDto<GetSupervisionCohort> result = await this._service.GetSupervisionCohort(id);
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo(SuccessMessages.DefaultSuccess));
        });
        Assert.That(result.Result, Is.Not.Null);
        Assert.That(result.Result!.Id, Is.EqualTo(id));
    }

    [Test]
    public async Task GetSupervisionCohorts_ReturnsPaginatedList()
    {
        // Arrange
        var parameters = new SupervisionCohortListParameters { /* ... populate with test data ... */ };
        var supervisionCohorts = new PagedList<SupervisionCohort>();
        var departmentList = new List<GetDepartment> { /* ... populate with test data ... */ };

        this._mockUnitOfWork.Setup(um => um.SupervisionCohortRepository.GetPaginatedListOfSupervisionCohort(parameters))
            .Returns(supervisionCohorts);
        this._mockDissertationApiService.Setup(service => service.GetAllDepartments())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetDepartment>> { IsSuccess = true, Result = departmentList });
        // Assume MapToSupervisionCohortDto is a method in SupervisionCohortService
        // this._mockMapper is not used since mapping is done internally in the service method

        // Act
        ResponseDto<PaginatedSupervisionCohortListDto> result = await this._service.GetSupervisionCohorts(parameters);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo(SuccessMessages.DefaultSuccess));
            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result?.Data, Is.Not.Null);
            Assert.That(result.Result!.TotalCount, Is.EqualTo(supervisionCohorts.TotalCount));
        });
    }

    [Test]
    public void GetActiveSupervisorsForCohort_ReturnsPaginatedList()
    {
        // Arrange
        var paginationParameters = new SupervisionCohortListParameters { /* ... populate with test data ... */ };
        var users = new List<ApplicationUser>
        {
            new ApplicationUser { /* ... populate with test data ... */ },
            new ApplicationUser { /* ... populate with test data ... */ }
        };
        const int pageNumber = 1;
        const int pageSize = 10;
        const int totalCount = 20; // Adjust based on the total number of items you want to simulate
        var pagedActiveSupervisors = new PagedList<ApplicationUser>(
            users, totalCount, pageNumber, pageSize);

        this._mockUnitOfWork.Setup(um => um.SupervisionCohortRepository.GetActiveSupervisors(paginationParameters))
            .Returns(pagedActiveSupervisors);

        // Act
        ResponseDto<PaginatedUserListDto> result = this._service.GetActiveSupervisorsForCohort(paginationParameters);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo(SuccessMessages.DefaultSuccess));
            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result!.TotalCount, Is.EqualTo(totalCount));
            Assert.That(result.Result.PageSize, Is.EqualTo(pageSize));
            Assert.That(result.Result.CurrentPage, Is.EqualTo(pageNumber));
        });
    }

    [Test]
    public void GetInActiveSupervisorsForCohort_ReturnsPaginatedList()
    {
        // Arrange
        var paginationParameters = new SupervisionCohortListParameters { /* ... populate with test data ... */ };
        var users = new List<ApplicationUser>
        {
            new ApplicationUser { /* ... populate with test data ... */ },
            new ApplicationUser { /* ... populate with test data ... */ }
        };
        var inactiveSupervisors = new PagedList<ApplicationUser>(
            users, 1, 10, 2);

        this._mockUnitOfWork.Setup(um => um.SupervisionCohortRepository.GetInActiveSupervisors(paginationParameters))
            .Returns(inactiveSupervisors);

        // Act
        ResponseDto<PaginatedUserListDto> result = this._service.GetInActiveSupervisorsForCohort(paginationParameters);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo(SuccessMessages.DefaultSuccess));
            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result!.TotalCount, Is.EqualTo(inactiveSupervisors.TotalCount));
            Assert.That(result.Result.PageSize, Is.EqualTo(inactiveSupervisors.PageSize));
            Assert.That(result.Result.CurrentPage, Is.EqualTo(inactiveSupervisors.CurrentPage));
            Assert.That(result.Result.TotalPages, Is.EqualTo(inactiveSupervisors.TotalPages));
            Assert.That(result.Result.HasNext, Is.EqualTo(inactiveSupervisors.HasNext));
            Assert.That(result.Result.HasPrevious, Is.EqualTo(inactiveSupervisors.HasPrevious));
        });
    }

    [Test]
    public async Task DeleteSupervisionCohort_FailedDueToAcceptedRequests()
    {
        // Arrange
        long supervisionCohortId = 1;
        var supervisionCohort = SupervisionCohort.Create(
            "supervisor",
            2,
            20402
        );
        supervisionCohort.Id = supervisionCohortId;
        supervisionCohort.SupervisionSlot = 5;
        supervisionCohort.AvailableSupervisionSlot = 3;

        this._mockUnitOfWork.Setup(um => um.SupervisionCohortRepository.GetAsync(It.IsAny<Expression<Func<SupervisionCohort, bool>>>(), null))
            .ReturnsAsync(supervisionCohort);

        // Act
        ResponseDto<string> result = await this._service.DeleteSupervisionCohort(supervisionCohortId, new CancellationToken());
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("This supervisor has already accepted a request already. Please reduce the slots instead"));
        });
    }

    [Test]
    public void DeleteSupervisionCohort_NotFound_ThrowsNotFoundException()
    {
        // Arrange
        const long supervisionCohortId = 1;
        this._mockUnitOfWork.Setup(um => um.SupervisionCohortRepository.GetAsync(It.IsAny<Expression<Func<SupervisionCohort, bool>>>(), null, null))
            .ReturnsAsync((SupervisionCohort)null!);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await this._service.DeleteSupervisionCohort(supervisionCohortId, new CancellationToken()));
    }
}