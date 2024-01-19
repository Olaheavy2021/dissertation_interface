using Moq;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Helpers;
using Shared.Logging;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;
using UserManagement_API.Service;
using UserManagement_API.Service.IService;

namespace UnitTests.UserManagementAPI.Service;

public class SupervisionListServiceTest
{
    private Mock<IAppLogger<SupervisionListService>> _mockLogger = null!;
    private Mock<IUnitOfWork> _mockUnitOfWork = null!;
    private Mock<IDissertationApiService> _dissertationApi = null!;
    private SupervisionListService _supervisionListService = null!;

    [SetUp]
    public void Setup()
    {
        this._mockLogger = new Mock<IAppLogger<SupervisionListService>>();
        this._mockUnitOfWork = new Mock<IUnitOfWork>();
        this._dissertationApi = new Mock<IDissertationApiService>();
        this._supervisionListService = new SupervisionListService(this._mockUnitOfWork.Object, this._mockLogger.Object,
            this._dissertationApi.Object);
    }

    [Test]
    public void GetPaginatedListOfSupervisionRequest_ShouldThrowNotFoundException_WhenDepartmentsNotFound()
    {
        // Arrange
        var parameters = new SupervisionListPaginationParameters();
        var supervisionLists = new PagedList<SupervisionList>();
        this._mockUnitOfWork.Setup(unitOfWork => unitOfWork.SupervisionListRepository.GetPaginatedListOfSupervisionLists(parameters))
            .Returns(supervisionLists);
        this._dissertationApi.Setup(service => service.GetAllDepartments())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetDepartment>> { Result = null });

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await this._supervisionListService.GetPaginatedListOfSupervisionRequest(parameters));
    }

    [Test]
    public void GetPaginatedListOfSupervisionRequest_ShouldThrowNotFoundException_WhenCoursesNotFound()
    {
        // Arrange
        var parameters = new SupervisionListPaginationParameters { /* ... populate parameters ... */ };
        var supervisionLists = new PagedList<SupervisionList>();
        this._mockUnitOfWork.Setup(unitOfWork => unitOfWork.SupervisionListRepository.GetPaginatedListOfSupervisionLists(parameters))
            .Returns(supervisionLists);
        this._dissertationApi.Setup(service => service.GetAllCourses())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetCourse>> { Result = null });
        this._dissertationApi.Setup(service => service.GetAllDepartments())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetDepartment>> { Result = new List<GetDepartment> { /* ... populate departments ... */ } });

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await this._supervisionListService.GetPaginatedListOfSupervisionRequest(parameters));
    }

    [Test]
    public async Task GetPaginatedListOfSupervisionRequest_ShouldReturnPaginatedSupervisionRequestsSuccessfully()
    {
        // Arrange
        var parameters = new SupervisionListPaginationParameters { /* ... populate parameters ... */ };
        var supervisionLists = new PagedList<SupervisionList>(/* ... populate supervisionLists ... */);
        var departments = new List<GetDepartment> { /* ... populate departments ... */ };
        var courses = new List<GetCourse> { /* ... populate courses ... */ };

        this._mockUnitOfWork.Setup(unitOfWork => unitOfWork.SupervisionListRepository.GetPaginatedListOfSupervisionLists(parameters))
            .Returns(supervisionLists);
        this._dissertationApi.Setup(service => service.GetAllDepartments())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetDepartment>> { Result = departments });
        this._dissertationApi.Setup(service => service.GetAllCourses())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetCourse>> { Result = courses });

        // Act
        ResponseDto<PaginatedSupervisionListDto> result = await this._supervisionListService.GetPaginatedListOfSupervisionRequest(parameters);
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo(SuccessMessages.DefaultSuccess));
            Assert.That(result.Result, Is.Not.Null);
        });
    }

    [Test]
    public void GetPaginatedListOfSupervisionListForAStudent_ShouldThrowNotFoundException_WhenDepartmentsNotFound()
    {
        // Arrange
        var parameters = new SupervisionListPaginationParameters();
        var supervisionLists = new PagedList<SupervisionList>();
        this._mockUnitOfWork.Setup(unitOfWork => unitOfWork.SupervisionListRepository.GetSupervisionListsForStudent(parameters))
            .Returns(supervisionLists);
        this._dissertationApi.Setup(service => service.GetAllDepartments())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetDepartment>> { Result = null });

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await this._supervisionListService.GetPaginatedListOfSupervisionListForAStudent(parameters));
    }

    [Test]
    public void GetPaginatedListOfSupervisionListForAStudent_ShouldThrowNotFoundException_WhenCoursesNotFound()
    {
        // Arrange
        var parameters = new SupervisionListPaginationParameters { /* ... populate parameters ... */ };
        var supervisionLists = new PagedList<SupervisionList>();
        this._mockUnitOfWork.Setup(unitOfWork => unitOfWork.SupervisionListRepository.GetSupervisionListsForStudent(parameters))
            .Returns(supervisionLists);
        this._dissertationApi.Setup(service => service.GetAllCourses())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetCourse>> { Result = null });
        this._dissertationApi.Setup(service => service.GetAllDepartments())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetDepartment>> { Result = new List<GetDepartment> { /* ... populate departments ... */ } });

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await this._supervisionListService.GetPaginatedListOfSupervisionListForAStudent(parameters));
    }

    [Test]
    public async Task GetPaginatedListOfSupervisionListForAStudent_ShouldReturnPaginatedSupervisionListsSuccessfully()
    {
        // Arrange
        var parameters = new SupervisionListPaginationParameters { /* ... populate parameters ... */ };
        var supervisionLists = new PagedList<SupervisionList>(/* ... populate supervisionLists ... */);
        var departments = new List<GetDepartment> { /* ... populate departments ... */ };
        var courses = new List<GetCourse> { /* ... populate courses ... */ };

        this._mockUnitOfWork.Setup(unitOfWork => unitOfWork.SupervisionListRepository.GetSupervisionListsForStudent(parameters))
            .Returns(supervisionLists);
        this._dissertationApi.Setup(service => service.GetAllDepartments())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetDepartment>> { Result = departments });
        this._dissertationApi.Setup(service => service.GetAllCourses())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetCourse>> { Result = courses });

        // Act
        ResponseDto<PaginatedSupervisionListDto> result = await this._supervisionListService.GetPaginatedListOfSupervisionListForAStudent(parameters);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo(SuccessMessages.DefaultSuccess));
            Assert.That(result.Result, Is.Not.Null);
            // ... Assertions to validate the paginated result, similar to previous tests ...
        });
    }

    [Test]
    public void GetPaginatedListOfSupervisionListForASupervisor_ShouldThrowNotFoundException_WhenDepartmentsNotFound()
    {
        // Arrange
        var parameters = new SupervisionListPaginationParameters();
        var supervisionLists = new PagedList<SupervisionList>();
        this._mockUnitOfWork.Setup(unitOfWork => unitOfWork.SupervisionListRepository.GetSupervisionListsForSupervisor(parameters))
            .Returns(supervisionLists);
        this._dissertationApi.Setup(service => service.GetAllDepartments())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetDepartment>> { Result = null });

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await this._supervisionListService.GetPaginatedListOfSupervisionListForASupervisor(parameters));
    }

    [Test]
    public void GetPaginatedListOfSupervisionListForASupervisor_ShouldThrowNotFoundException_WhenCoursesNotFound()
    {
        // Arrange
        var parameters = new SupervisionListPaginationParameters { /* ... populate parameters ... */ };
        var supervisionLists = new PagedList<SupervisionList>();
        this._mockUnitOfWork.Setup(unitOfWork => unitOfWork.SupervisionListRepository.GetSupervisionListsForSupervisor(parameters))
            .Returns(supervisionLists);
        this._dissertationApi.Setup(service => service.GetAllCourses())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetCourse>> { Result = null });
        this._dissertationApi.Setup(service => service.GetAllDepartments())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetDepartment>> { Result = new List<GetDepartment> { /* ... populate departments ... */ } });

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await this._supervisionListService.GetPaginatedListOfSupervisionListForASupervisor(parameters));
    }

    [Test]
    public async Task GetPaginatedListOfSupervisionListForASupervisor_ShouldReturnPaginatedSupervisionListsSuccessfully()
    {
        // Arrange
        var parameters = new SupervisionListPaginationParameters { /* ... populate parameters ... */ };
        var supervisionLists = new PagedList<SupervisionList>(/* ... populate supervisionLists ... */);
        var departments = new List<GetDepartment> { /* ... populate departments ... */ };
        var courses = new List<GetCourse> { /* ... populate courses ... */ };

        this._mockUnitOfWork.Setup(unitOfWork => unitOfWork.SupervisionListRepository.GetSupervisionListsForSupervisor(parameters))
            .Returns(supervisionLists);
        this._dissertationApi.Setup(service => service.GetAllDepartments())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetDepartment>> { Result = departments });
        this._dissertationApi.Setup(service => service.GetAllCourses())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetCourse>> { Result = courses });

        // Act
        ResponseDto<PaginatedSupervisionListDto> result = await this._supervisionListService.GetPaginatedListOfSupervisionListForASupervisor(parameters);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo(SuccessMessages.DefaultSuccess));
            Assert.That(result.Result, Is.Not.Null);
        });
    }






}