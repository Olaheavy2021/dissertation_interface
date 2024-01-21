using System.Collections.ObjectModel;
using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Shared.Constants;
using Shared.DTO;
using Shared.Enums;
using Shared.Exceptions;
using Shared.Helpers;
using Shared.MessageBus;
using Shared.Settings;
using UnitTests.UserManagementAPI.Mocks;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;
using UserManagement_API.Service;
using UserManagement_API.Service.IService;

namespace UnitTests.UserManagementAPI.Service;

public class SupervisionRequestTest
{
    private Mock<IUnitOfWork> _mockUnitOfWork = null!;
    private Mock<ISupervisionCohortService> _mockSupervisionCohortService = null!;
    private Mock<IDissertationApiService> _mockDissertationApiService = null!;
    private Mock<IHttpContextAccessor> _mockHttpContextAccessor = null!;
    private Mock<IMapper> _mockMapper = null!;
    private Mock<ILogger<SupervisionRequestService>> _mockLogger = null!;
    private Mock<IMessageBus> _mockMessageBus = null!;
    private Mock<IOptions<ServiceBusSettings>> _mockServiceBusSettings = null!;
    private ServiceBusSettings _serviceBusSettingsValue = new();
    private ApplicationUser? _applicationUser = new();
    private UserDto _userDto = new();
    private SupervisionRequestService _supervisionRequestService = null!;

    [SetUp]
    public void Setup()
    {
        this._mockUnitOfWork = new Mock<IUnitOfWork>();
        this._mockSupervisionCohortService = new Mock<ISupervisionCohortService>();
        this._mockDissertationApiService = new Mock<IDissertationApiService>();
        this._mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        this._mockMapper = new Mock<IMapper>();
        this._mockLogger = new Mock<ILogger<SupervisionRequestService>>();
        this._mockMessageBus = new Mock<IMessageBus>();
        this._mockServiceBusSettings = new Mock<IOptions<ServiceBusSettings>>();
        this._mockServiceBusSettings.Setup(settings => settings.Value).Returns(this._serviceBusSettingsValue); // Assuming you have some default values for settings

        this._supervisionRequestService = new SupervisionRequestService(
            this._mockUnitOfWork.Object,
            this._mockSupervisionCohortService.Object,
            this._mockDissertationApiService.Object,
            this._mockHttpContextAccessor.Object,
            this._mockMapper.Object,
            this._mockLogger.Object,
            this._mockMessageBus.Object,
            this._mockServiceBusSettings.Object // Pass the Value of the IOptions
        );

        #region TestData
        this._applicationUser = TestData.User;
        this._applicationUser.ProfilePicture = TestData.ProfilePicture;
        this._serviceBusSettingsValue = TestData.ServiceBusSettings;
        this._userDto = TestData.UserDtoResponse;
        #endregion
    }

    [Test]
    public async Task CreateSupervisionRequest_ShouldReturnFailure_WhenNoActiveDissertationCohort()
    {
        // Arrange
        var request = new CreateSupervisionRequest { /* ... populate request data ... */ };
        this._mockDissertationApiService.Setup(service => service.GetActiveDissertationCohort())
            .ReturnsAsync(new ResponseDto<GetDissertationCohort> { Result = null });

        // Act
        ResponseDto<string> result = await this._supervisionRequestService.CreateSupervisionRequest(request, new CancellationToken());
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("There is currently no Active Dissertation Cohort"));
        });
    }

    [Test]
    public async Task CreateSupervisionRequest_ShouldReturnFailure_WhenDeadlineHasPassed()
    {
        // Arrange
        var request = new CreateSupervisionRequest { /* ... populate request data ... */ };
        var dissertationCohort = new GetDissertationCohort { SupervisionChoiceDeadline = DateTime.UtcNow.AddDays(-1) };
        this._mockDissertationApiService.Setup(service => service.GetActiveDissertationCohort())
            .ReturnsAsync(new ResponseDto<GetDissertationCohort> { Result = dissertationCohort });

        // Act
        ResponseDto<string> result = await this._supervisionRequestService.CreateSupervisionRequest(request, new CancellationToken());
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("The supervision request deadline has passed for this dissertation cohort"));
        });
    }

    [Test]
    public async Task CreateSupervisionRequest_ShouldReturnFailure_WhenSupervisionCohortDoesNotExistOrSlotExceeded()
    {
        // Arrange
        var request = new CreateSupervisionRequest
        {
            SupervisorId = this._applicationUser?.Id!
        };
        var dissertationCohort = new GetDissertationCohort
        {
            SupervisionChoiceDeadline = DateTime.UtcNow.AddDays(5)
        };
        this._mockDissertationApiService.SetupSequence(service => service.GetActiveDissertationCohort())
            .ReturnsAsync(new ResponseDto<GetDissertationCohort> { Result = dissertationCohort })
            .ReturnsAsync(new ResponseDto<GetDissertationCohort> { Result = dissertationCohort }); // If called more than once
        this._mockSupervisionCohortService.Setup(service => service.GetSupervisionCohort(It.IsAny<SupervisionCohortParameters>()))
            .ReturnsAsync(new ResponseDto<SupervisionCohort> { Result = SupervisionCohort.Create(this._applicationUser?.Id!, 0, 20442), IsSuccess = true });

        // Act
        ResponseDto<string> result = await this._supervisionRequestService.CreateSupervisionRequest(request, new CancellationToken());
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("There are no supervision slots available for this supervisor"));
        });
    }

    [Test]
    public async Task CreateSupervisionRequest_ShouldReturnFailure_WhenSupervisionCohortDoesNotExist()
    {
        // Arrange
        var request = new CreateSupervisionRequest
        {
            SupervisorId = this._applicationUser?.Id!
        };
        var dissertationCohort = new GetDissertationCohort
        {
            SupervisionChoiceDeadline = DateTime.UtcNow.AddDays(5)
        };
        this._mockDissertationApiService.SetupSequence(service => service.GetActiveDissertationCohort())
            .ReturnsAsync(new ResponseDto<GetDissertationCohort> { Result = dissertationCohort })
            .ReturnsAsync(new ResponseDto<GetDissertationCohort> { Result = dissertationCohort }); // If called more than once
        this._mockSupervisionCohortService.Setup(service => service.GetSupervisionCohort(It.IsAny<SupervisionCohortParameters>()))
            .ReturnsAsync(new ResponseDto<SupervisionCohort> { Result = null!, IsSuccess = false });

        // Act
        ResponseDto<string> result = await this._supervisionRequestService.CreateSupervisionRequest(request, new CancellationToken());
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Result, Is.EqualTo(ErrorMessages.DefaultError));
        });
    }

    [Test]
    public async Task CreateSupervisionRequest_ShouldReturnFailure_WhenStudentHasThreeActiveRequests()
    {
        // Arrange
        var request = new CreateSupervisionRequest { /* ... populate request data ... */ };
        SetupDissertationApiWithActiveCohort();
        SetupHttpContextWithStudentId();
        SetupSupervisionCohortServiceWithAvailableSlot();
        SetupSupervisionRequestRepositoryWithActiveRequests(3);

        // Act
        ResponseDto<string> result = await this._supervisionRequestService.CreateSupervisionRequest(request, new CancellationToken());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("You can only have 3 pending requests at a time"));
        });
    }

    [Test]
    public async Task CreateSupervisionRequest_ShouldReturnFailure_WhenStudentIdIsNull()
    {
        // Arrange
        var request = new CreateSupervisionRequest { /* ... populate request data ... */ };
        SetupDissertationApiWithActiveCohort();
        var context = new DefaultHttpContext {  Items = { ["UserId"] = string.Empty }};
        this._mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(context);
        SetupSupervisionCohortServiceWithAvailableSlot();
        SetupSupervisionRequestRepositoryWithActiveRequests(3);

        // Act
        ResponseDto<string> result = await this._supervisionRequestService.CreateSupervisionRequest(request, new CancellationToken());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Invalid Request"));
        });
    }

    [Test]
    public async Task CreateSupervisionRequest_ShouldReturnFailure_WhenStudentHasPendingRequestWithSupervisor()
    {
        // Arrange
        var request = new CreateSupervisionRequest { /* ... populate request data ... */ };
        SetupDissertationApiWithActiveCohort();
        SetupHttpContextWithStudentId();
        SetupSupervisionCohortServiceWithAvailableSlot();
        SetupCheckIfStudentHasAPendingRequestWithThisSupervisor(true);

        // Act
        ResponseDto<string> result = await this._supervisionRequestService.CreateSupervisionRequest(request, new CancellationToken());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("You have a pending request sent to this supervisor"));
        });
    }

    [Test]
    public async Task CreateSupervisionRequest_ShouldReturnFailure_WhenStudentIsPairedAlready()
    {
        // Arrange
        var request = new CreateSupervisionRequest { /* ... populate request data ... */ };
        SetupDissertationApiWithActiveCohort();
        SetupHttpContextWithStudentId();
        SetupSupervisionCohortServiceWithAvailableSlot();
        SetupCheckIfStudentHasAPendingRequestWithThisSupervisor(false);
        SetupCheckIfStudentHasASupervisor(true);
        // Simulate student is paired with a supervisor

        // Act
        ResponseDto<string> result = await this._supervisionRequestService.CreateSupervisionRequest(request, new CancellationToken());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("This Student is paired to a supervisor already"));
        });
    }

    [Test]
    public async Task CreateSupervisionRequest_ShouldReturnFailure_WhenSupervisorIsDisabled()
    {
        // Arrange
        var request = new CreateSupervisionRequest { /* ... populate request data ... */ };
        SetupDissertationApiWithActiveCohort();
        SetupHttpContextWithStudentId();
        SetupSupervisionCohortServiceWithAvailableSlot();
        SetupCheckIfStudentHasAPendingRequestWithThisSupervisor(false);
        SetupCheckIfStudentHasASupervisor(false);
        SetupSupervisionCohortServiceWithDisabledSupervisor();

        // Act
        ResponseDto<string> result = await this._supervisionRequestService.CreateSupervisionRequest(request, new CancellationToken());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("The supervisor is currently disabled by the admin"));
        });
    }

    [Test]
    public async Task CreateSupervisionRequest_ShouldReturnSuccess_WhenRequestInitiatedSuccessfully()
    {
        // Arrange
        var request = new CreateSupervisionRequest { /* ... populate request data ... */ };
        SetupMocksForSuccessfulScenario();

        // Act
        ResponseDto<string> result = await this._supervisionRequestService.CreateSupervisionRequest(request, new CancellationToken());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Supervision Request initiated Successfully"));
        });
    }

    [Test]
    public void GetPaginatedListOfSupervisionRequest_ShouldThrowNotFoundException_WhenDepartmentsNotFound()
    {
        // Arrange
        var parameters = new SupervisionRequestPaginationParameters { /* ... populate parameters ... */ };
        var supervisionRequests = new PagedList<SupervisionRequest>(/* ... populate supervisionRequests ... */);
        this._mockUnitOfWork.Setup(unitOfWork => unitOfWork.SupervisionRequestRepository.GetPaginatedListOfSupervisionRequests(parameters))
            .Returns(supervisionRequests);
        this._mockDissertationApiService.Setup(service => service.GetAllDepartments())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetDepartment>> { Result = null });

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await this._supervisionRequestService.GetPaginatedListOfSupervisionRequest(parameters));
    }

    [Test]
    public void GetPaginatedListOfSupervisionRequest_ShouldThrowNotFoundException_WhenCoursesNotFound()
    {
        // Arrange
        var parameters = new SupervisionRequestPaginationParameters { /* ... populate parameters ... */ };
        var supervisionRequests = new PagedList<SupervisionRequest>(/* ... populate supervisionRequests ... */);
        this._mockUnitOfWork.Setup(unitOfWork => unitOfWork.SupervisionRequestRepository.GetPaginatedListOfSupervisionRequests(parameters))
            .Returns(supervisionRequests);
        this._mockDissertationApiService.Setup(service => service.GetAllDepartments())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetDepartment>> { Result = new List<GetDepartment> { /* ... populate departments ... */ } });
        this._mockDissertationApiService.Setup(service => service.GetAllCourses())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetCourse>> { Result = null });

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await this._supervisionRequestService.GetPaginatedListOfSupervisionRequest(parameters));
    }

    [Test]
    public async Task GetPaginatedListOfSupervisionRequest_ShouldReturnPaginatedSupervisionRequestsSuccessfully()
    {
        // Arrange
        var parameters = new SupervisionRequestPaginationParameters { /* ... populate parameters ... */ };
        var supervisionRequests = new PagedList<SupervisionRequest>(/* ... populate supervisionRequests ... */);
        var departments = new List<GetDepartment> { /* ... populate departments ... */ };
        var courses = new List<GetCourse> { /* ... populate courses ... */ };

        this._mockUnitOfWork.Setup(unitOfWork => unitOfWork.SupervisionRequestRepository.GetPaginatedListOfSupervisionRequests(parameters))
            .Returns(supervisionRequests);
        this._mockDissertationApiService.Setup(service => service.GetAllDepartments())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetDepartment>> { Result = departments });
        this._mockDissertationApiService.Setup(service => service.GetAllCourses())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetCourse>> { Result = courses });

        // Act
        ResponseDto<PaginatedSupervisionRequestListDto> result = await this._supervisionRequestService.GetPaginatedListOfSupervisionRequest(parameters);

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
    public void GetPaginatedListOfSupervisionRequestForAStudent_ShouldThrowNotFoundException_WhenDepartmentsNotFound()
    {
        // Arrange
        var parameters = new SupervisionRequestPaginationParameters { /* ... populate parameters ... */ };
        this._mockUnitOfWork.Setup(unitOfWork => unitOfWork.SupervisionRequestRepository.GetStudentListOfSupervisionRequests(parameters))
            .Returns(new PagedList<SupervisionRequest>(/* ... populate supervisionRequests ... */));
        this._mockDissertationApiService.Setup(service => service.GetAllDepartments())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetDepartment>> { Result = null });

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await this._supervisionRequestService.GetPaginatedListOfSupervisionRequestForAStudent(parameters));
    }

    [Test]
    public void GetPaginatedListOfSupervisionRequestForAStudent_ShouldThrowNotFoundException_WhenCoursesNotFound()
    {
        // Arrange
        var parameters = new SupervisionRequestPaginationParameters { /* ... populate parameters ... */ };
        this._mockUnitOfWork.Setup(unitOfWork => unitOfWork.SupervisionRequestRepository.GetStudentListOfSupervisionRequests(parameters))
            .Returns(new PagedList<SupervisionRequest>(/* ... populate supervisionRequests ... */));
        this._mockDissertationApiService.Setup(service => service.GetAllDepartments())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetDepartment>> { Result = new List<GetDepartment> { /* ... populate departments ... */ } });
        this._mockDissertationApiService.Setup(service => service.GetAllCourses())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetCourse>> { Result = null });

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await this._supervisionRequestService.GetPaginatedListOfSupervisionRequestForAStudent(parameters));
    }

    [Test]
    public async Task GetPaginatedListOfSupervisionRequestForAStudent_ShouldReturnPaginatedSupervisionRequestsSuccessfully()
    {
        // Arrange
        var parameters = new SupervisionRequestPaginationParameters { /* ... populate parameters ... */ };
        var supervisionRequests = new PagedList<SupervisionRequest>(/* ... populate supervisionRequests ... */);
        var departments = new List<GetDepartment> { /* ... populate departments ... */ };
        var courses = new List<GetCourse> { /* ... populate courses ... */ };

        this._mockUnitOfWork.Setup(unitOfWork => unitOfWork.SupervisionRequestRepository.GetStudentListOfSupervisionRequests(parameters))
            .Returns(supervisionRequests);
        this._mockDissertationApiService.Setup(service => service.GetAllDepartments())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetDepartment>> { Result = departments });
        this._mockDissertationApiService.Setup(service => service.GetAllCourses())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetCourse>> { Result = courses });

        // Act
        ResponseDto<PaginatedSupervisionRequestListDto> result = await this._supervisionRequestService.GetPaginatedListOfSupervisionRequestForAStudent(parameters);

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
    public void GetPaginatedListOfSupervisionRequestForASupervisor_ShouldThrowNotFoundException_WhenDepartmentsNotFound()
    {
        // Arrange
        var parameters = new SupervisionRequestPaginationParameters { /* ... populate parameters ... */ };
        this._mockUnitOfWork.Setup(unitOfWork => unitOfWork.SupervisionRequestRepository.GetSupervisorListOfSupervisionRequests(parameters))
            .Returns(new PagedList<SupervisionRequest>(/* ... populate supervisionRequests ... */));
        this._mockDissertationApiService.Setup(service => service.GetAllDepartments())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetDepartment>> { Result = null });

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await this._supervisionRequestService.GetPaginatedListOfSupervisionRequestForASupervisor(parameters));
    }

    [Test]
    public void GetPaginatedListOfSupervisionRequestForASupervisor_ShouldThrowNotFoundException_WhenCoursesNotFound()
    {
        // Arrange
        var parameters = new SupervisionRequestPaginationParameters { /* ... populate parameters ... */ };
        this._mockUnitOfWork.Setup(unitOfWork => unitOfWork.SupervisionRequestRepository.GetSupervisorListOfSupervisionRequests(parameters))
            .Returns(new PagedList<SupervisionRequest>(/* ... populate supervisionRequests ... */));
        this._mockDissertationApiService.Setup(service => service.GetAllDepartments())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetDepartment>> { Result = new List<GetDepartment> { /* ... populate departments ... */ } });
        this._mockDissertationApiService.Setup(service => service.GetAllCourses())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetCourse>> { Result = null });

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await this._supervisionRequestService.GetPaginatedListOfSupervisionRequestForASupervisor(parameters));
    }

    [Test]
    public async Task GetPaginatedListOfSupervisionRequestForASupervisor_ShouldReturnPaginatedSupervisionRequestsSuccessfully()
    {
        // Arrange
        var parameters = new SupervisionRequestPaginationParameters { /* ... populate parameters ... */ };
        var supervisionRequests = new PagedList<SupervisionRequest>(/* ... populate supervisionRequests ... */);
        var departments = new List<GetDepartment> { /* ... populate departments ... */ };
        var courses = new List<GetCourse> { /* ... populate courses ... */ };

        this._mockUnitOfWork.Setup(unitOfWork => unitOfWork.SupervisionRequestRepository.GetSupervisorListOfSupervisionRequests(parameters))
            .Returns(supervisionRequests);
        this._mockDissertationApiService.Setup(service => service.GetAllDepartments())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetDepartment>> { Result = departments });
        this._mockDissertationApiService.Setup(service => service.GetAllCourses())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetCourse>> { Result = courses });

        // Act
        ResponseDto<PaginatedSupervisionRequestListDto> result = await this._supervisionRequestService.GetPaginatedListOfSupervisionRequestForASupervisor(parameters);

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
    public async Task CancelSupervisionRequest_ShouldReturnFailure_WhenRequestDoesNotMatchStudent()
    {
        // Arrange
        var request = new ActionSupervisionRequest { /* ... populate request data ... */ };
        SetupHttpContextWithStudentId("otherStudentId");
        SetupSupervisionRequestRepositoryWithRequest(supervisorId: "supervisor");

        // Act
        ResponseDto<string> result = await this._supervisionRequestService.CancelSupervisionRequest(request, new CancellationToken());
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("You are not authorized to action this request"));
        });
    }

    [Test]
    public async Task CancelSupervisionRequest_ShouldReturnFailure_WhenRequestIsNotPending()
    {
        // Arrange
        var request = new ActionSupervisionRequest { /* ... populate request data ... */ };
        SetupHttpContextWithStudentId("student");
        SetupSupervisionRequestRepositoryWithRequest(SupervisionRequestStatus.Approved); // Simulate request is not pending

        // Act
        ResponseDto<string> result = await this._supervisionRequestService.CancelSupervisionRequest(request, new CancellationToken());
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Supervision Request is not in the Pending State"));
        });
    }

    [Test]
    public async Task CancelSupervisionRequest_ShouldReturnSuccess_WhenRequestCancelledSuccessfully()
    {
        // Arrange
        var request = new ActionSupervisionRequest { /* ... populate request data ... */ };
        SetupHttpContextWithStudentId("student");
        SetupSupervisionRequestRepositoryWithRequest(supervisorId: "supervisor"); // Simulate request is not pending
        this._mockUnitOfWork.Setup(unitOfWork => unitOfWork.SupervisionRequestRepository.Remove(It.IsAny<SupervisionRequest>()))
            .Verifiable();
        this._mockUnitOfWork.Setup(unitOfWork => unitOfWork.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        ResponseDto<string> result = await this._supervisionRequestService.CancelSupervisionRequest(request, new CancellationToken());
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Supervision Request cancelled successfully"));
        });
        this._mockUnitOfWork.Verify(unitOfWork => unitOfWork.SupervisionRequestRepository.Remove(It.IsAny<SupervisionRequest>()), Times.Once);
        this._mockUnitOfWork.Verify(unitOfWork => unitOfWork.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task RejectSupervisionRequest_ShouldReturnFailure_WhenRequestDoesNotMatchSupervisor()
    {
        // Arrange
        var request = new ActionSupervisionRequest { /* ... populate request data ... */ };
        SetupHttpContextWithSupervisorId("otherSupervisorId");
        SetupSupervisionRequestRepositoryWithRequest(supervisorId: "differentSupervisorId");

        // Act
        ResponseDto<string> result = await this._supervisionRequestService.RejectSupervisionRequest(request, new CancellationToken());

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Message, Is.EqualTo("You are not authorized to action this request"));
    }

    [Test]
    public async Task RejectSupervisionRequest_ShouldReturnFailure_WhenRequestIsNotPending()
    {
        // Arrange
        var request = new ActionSupervisionRequest { /* ... populate request data ... */ };
        SetupHttpContextWithSupervisorId();
        SetupSupervisionRequestRepositoryWithRequest(supervisorId: "supervisorId", SupervisionRequestStatus.Approved); // Simulate request is not pending

        // Act
        ResponseDto<string> result = await this._supervisionRequestService.RejectSupervisionRequest(request, new CancellationToken());

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Message, Is.EqualTo("Supervision Request is not in the Pending State"));
    }

    [Test]
    public async Task RejectSupervisionRequest_ShouldReturnSuccess_WhenRequestRejectedSuccessfully()
    {
        // Arrange
        var request = new ActionSupervisionRequest { /* ... populate request data ... */ };
        SetupHttpContextWithSupervisorId();
        SetupSupervisionRequestRepositoryWithRequest(supervisorId: "supervisorId");
        this._mockUnitOfWork.Setup(unitOfWork => unitOfWork.SupervisionRequestRepository.Update(It.IsAny<SupervisionRequest>()))
            .Verifiable();
        this._mockUnitOfWork.Setup(unitOfWork => unitOfWork.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();
        this._mockMapper.Setup(mapper => mapper.Map<ApplicationUser, UserDto>(It.IsAny<ApplicationUser>())).Returns(this._userDto);

        // Act
        ResponseDto<string> result = await this._supervisionRequestService.RejectSupervisionRequest(request, new CancellationToken());

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Message, Is.EqualTo("Supervision Request rejected successfully"));
        this._mockUnitOfWork.Verify(unitOfWork => unitOfWork.SupervisionRequestRepository.Update(It.IsAny<SupervisionRequest>()), Times.Once);
        this._mockUnitOfWork.Verify(unitOfWork => unitOfWork.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task AcceptSupervisionRequest_ShouldReturnFailure_WhenNotAuthorized()
    {
        // Arrange
        var request = new ActionSupervisionRequest
        {
            RequestId = 23455
        };
        SetupSupervisionRequestRepositoryWithRequest(supervisorId: "differentSupervisorId");
        SetupHttpContextWithSupervisorId("supervisor");
        SetupSupervisionCohortMock(2);

        // Act
        ResponseDto<string> result = await this._supervisionRequestService.AcceptSupervisionRequest(request, new CancellationToken());
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("You are not authorized to action this request"));
        });
    }

    [Test]
    public async Task AcceptSupervisionRequest_ShouldReturnFailure_WhenRequestIsNotPending()
    {
        // Arrange
        var request = new ActionSupervisionRequest { RequestId = 23455 };
        SetupHttpContextWithSupervisorId("supervisor");
        SetupSupervisionRequestRepositoryWithRequest(supervisorId: "supervisor", SupervisionRequestStatus.Approved);
        SetupSupervisionCohortMock(2);

        // Act
        ResponseDto<string> result = await this._supervisionRequestService.AcceptSupervisionRequest(request, new CancellationToken());
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Supervision Request is not in the Pending State"));
        });
    }

    [Test]
    public async Task AcceptSupervisionRequest_ShouldReturnFailure_WhenNoAvailableSlots()
    {
        // Arrange
        var request = new ActionSupervisionRequest { /* populate request data */ };
        SetupHttpContextWithSupervisorId("supervisor");
        SetupSupervisionRequestRepositoryWithRequest(supervisorId: "supervisor");
        SetupSupervisionCohortMock(0);

        // Act
        ResponseDto<string> result = await this._supervisionRequestService.AcceptSupervisionRequest(request, new CancellationToken());
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("You have reached the maximum supervision slots allocated to you"));
        });
    }

    [Test]
    public async Task AcceptSupervisionRequest_ShouldReturnSuccess_WhenRequestAccepted()
    {
        // Arrange
        var request = new ActionSupervisionRequest { /* populate request data */ };
        SetupHttpContextWithSupervisorId("supervisor");
        SetupSupervisionRequestRepositoryWithRequest(supervisorId: "supervisor");
        SetupSupervisionCohortMock(2);
        this._mockUnitOfWork
            .Setup(repo => repo.SupervisionListRepository.AddAsync(It.IsAny<SupervisionList>()))
            .Returns(Task.CompletedTask) // As AddAsync is likely a void async method, we return a completed task
            .Verifiable();
        var studentSupervisionRequests = SupervisionRequest.Create(
            "supervisorId",
            "studentId",
            20222
        );
        ReadOnlyCollection<SupervisionRequest> supervisionRequests = new List<SupervisionRequest>()
        {
            studentSupervisionRequests
        }.AsReadOnly();


        this._mockUnitOfWork
            .Setup(repo => repo.SupervisionRequestRepository.GetAllAsync(
                It.IsAny<Expression<Func<SupervisionRequest, bool>>>(),
                null,
                null
            ))
            .ReturnsAsync(supervisionRequests)
            .Verifiable();

        // Act
        ResponseDto<string> result = await this._supervisionRequestService.AcceptSupervisionRequest(request, new CancellationToken());
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Supervision Request accepted successfully"));
        });
        this._mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private void SetupSupervisionRequestRepositoryWithRequest(SupervisionRequestStatus status = SupervisionRequestStatus.Pending)
    {
        var supervisionRequest = SupervisionRequest.Create(
            "supervisor",
            "student",
            20224
        );

        supervisionRequest.Status = status;
        supervisionRequest.Student = new ApplicationUser();
        supervisionRequest.Student = this._applicationUser!;

        this._mockUnitOfWork.Setup(unitOfWork => unitOfWork.SupervisionRequestRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<SupervisionRequest, bool>>>(),
                null,
                x => x.Student
                ))
            .ReturnsAsync(supervisionRequest);
    }

    private void SetupSupervisionRequestRepositoryWithRequest(string supervisorId = "supervisor", SupervisionRequestStatus status = SupervisionRequestStatus.Pending)
    {
        var supervisionRequest = SupervisionRequest.Create(
            supervisorId,
            "student",
            20224
        );

        supervisionRequest.Status = status;
        supervisionRequest.Student = new ApplicationUser();
        supervisionRequest.Student = this._applicationUser!;

        this._mockUnitOfWork.Setup(unitOfWork => unitOfWork.SupervisionRequestRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<SupervisionRequest, bool>>>(),
                null,
                x => x.Student
            ))
            .ReturnsAsync(supervisionRequest);
    }

    private void SetupDissertationApiWithActiveCohort()
    {
        var dissertationCohort = new GetDissertationCohort
        {
            Id = 1,
            SupervisionChoiceDeadline = DateTime.UtcNow.AddDays(5) // Set to a future date
        };
        this._mockDissertationApiService.Setup(service => service.GetActiveDissertationCohort())
            .ReturnsAsync(new ResponseDto<GetDissertationCohort> { Result = dissertationCohort });
    }

    private void SetupSupervisionCohortServiceWithAvailableSlot()
    {
        var supervisionCohort = SupervisionCohort.Create(this._applicationUser!.Id, 1, 2024);

        this._mockSupervisionCohortService.Setup(service => service.GetSupervisionCohort(It.IsAny<SupervisionCohortParameters>()))
            .ReturnsAsync(new ResponseDto<SupervisionCohort> { Result = supervisionCohort, IsSuccess = true });
    }

    private void SetupHttpContextWithStudentId(string studentId = "studentId")
    {
        var context = new DefaultHttpContext { Items = { ["UserId"] = studentId } };
        this._mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(context);
    }

    private void SetupHttpContextWithSupervisorId(string supervisorId = "supervisorId")
    {
        var context = new DefaultHttpContext { Items = { ["UserId"] = supervisorId } };
        this._mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(context);
    }

    private void SetupSupervisionRequestRepositoryWithActiveRequests(int activeRequestsCount) =>
        this._mockUnitOfWork.Setup(repo => repo.SupervisionRequestRepository.CountWhere(
                It.IsAny<Expression<Func<SupervisionRequest, bool>>>()))
            .ReturnsAsync(activeRequestsCount);

    private void SetupCheckIfStudentHasAPendingRequestWithThisSupervisor(bool hasPendingRequest) =>
        this._mockUnitOfWork.Setup(repo => repo.SupervisionRequestRepository.AnyAsync(It.IsAny<Expression<Func<SupervisionRequest, bool>>>()))
            .ReturnsAsync(hasPendingRequest);

    private void SetupCheckIfStudentHasASupervisor(bool isPairedAlready) =>
        this._mockUnitOfWork.Setup(repo => repo.SupervisionListRepository.AnyAsync(It.IsAny<Expression<Func<SupervisionList, bool>>>()))
            .ReturnsAsync(isPairedAlready);

    private void SetupSupervisionCohortServiceWithDisabledSupervisor()
    {
        var supervisionCohort = SupervisionCohort.Create(
            this._applicationUser?.Id!,
            3,
            20224
        );
        supervisionCohort.Supervisor = new ApplicationUser
        {
            IsLockedOutByAdmin = true
        };
        this._mockSupervisionCohortService.Setup(service => service.GetSupervisionCohort(It.IsAny<SupervisionCohortParameters>()))
            .ReturnsAsync(new ResponseDto<SupervisionCohort> { Result = supervisionCohort, IsSuccess = true });
    }

    private void SetupUnitOfWorkForSuccessfulSupervisionRequestCreation()
    {
        this._mockUnitOfWork.Setup(repo => repo.SupervisionRequestRepository.AddAsync(It.IsAny<SupervisionRequest>()))
            .Returns(Task.CompletedTask);
        this._mockUnitOfWork.Setup(repo => repo.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    private void SetupMocksForSuccessfulScenario()
    {
        // Set up an active dissertation cohort
        SetupDissertationApiWithActiveCohort();

        // Set up a supervision cohort with available slots
        SetupSupervisionCohortServiceWithAvailableSlot();

        // Set up a valid student ID in the HttpContext
        SetupHttpContextWithStudentId();

        // Simulate that the student has fewer than 3 active requests
        SetupSupervisionRequestRepositoryWithActiveRequests(2);

        // Simulate that the student does not have a pending request with this supervisor
        SetupCheckIfStudentHasAPendingRequestWithThisSupervisor(false);

        // Simulate that the student is not paired with a supervisor already
        SetupCheckIfStudentHasASupervisor(false);

        // Simulate that the supervisor is not disabled
        SetupSupervisionCohortServiceWithAvailableSupervisor();

        // Set up the UnitOfWork for successful supervision request creation
        SetupUnitOfWorkForSuccessfulSupervisionRequestCreation();
    }

    private void SetupSupervisionCohortServiceWithAvailableSupervisor()
    {
        var supervisionCohort = SupervisionCohort.Create(
            this._applicationUser?.Id!,
            3,
            20224
        );
        supervisionCohort.Supervisor = new ApplicationUser { IsLockedOutByAdmin = false };
        this._mockSupervisionCohortService.Setup(service => service.GetSupervisionCohort(It.IsAny<SupervisionCohortParameters>()))
            .ReturnsAsync(new ResponseDto<SupervisionCohort> { Result = supervisionCohort, IsSuccess = true });
    }

    private void SetupSupervisionCohortMock(int supervisionSlot)
    {
        var supervisorId = "supervisorId";
        var cohortId = 123; // Example cohortId
        var expectedSupervisionCohort = SupervisionCohort.Create(
            supervisorId,
            supervisionSlot,
            cohortId
        );

        this._mockUnitOfWork
            .Setup(repo => repo.SupervisionCohortRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<SupervisionCohort, bool>>>(),
                null,
                null,
                null
            ))
            .ReturnsAsync(expectedSupervisionCohort);
    }
}