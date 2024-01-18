using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Helpers;
using Shared.Logging;
using Shared.MessageBus;
using Shared.Settings;
using UnitTests.UserManagementAPI.Mocks;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;
using UserManagement_API.Data.Models.Dto;
using UserManagement_API.Helpers;
using UserManagement_API.Service.IService;

namespace UnitTests.UserManagementAPI.UserService;

public class GetPaginatedTests
{
    private Mock<IHttpContextAccessor> _httpContextAccessor = null!;
    private Mock<FakeUserManager> _userManager = null!;
    private Mock<IMapper> _mapper = null!;
    private Mock<IUnitOfWork> _mockUnitOfWork = null!;
    private Mock<IAppLogger<UserManagement_API.Service.UserService>> _logger = null!;
    private Mock<IOptions<ServiceBusSettings>> _serviceBusSettings = null!;
    private Mock<IMessageBus> _messageBus = null!;
    private Mock<IDissertationApiService> _dissertationApi = null!;
    private ApplicationUser? _applicationUser = new();
    private UserManagement_API.Service.UserService _userService = null!;



    [SetUp]
    public void Setup()
    {
        this._httpContextAccessor = new Mock<IHttpContextAccessor>();
        this._userManager = new Mock<FakeUserManager>();
        this._mapper = new Mock<IMapper>();
        this._logger = new Mock<IAppLogger<UserManagement_API.Service.UserService>>();
        this._mockUnitOfWork = new Mock<IUnitOfWork>();
        this._messageBus = new Mock<IMessageBus>();
        this._serviceBusSettings = new Mock<IOptions<ServiceBusSettings>>();
        this._dissertationApi = new Mock<IDissertationApiService>();
        this._userService = new UserManagement_API.Service.UserService(this._mockUnitOfWork.Object,this._logger.Object,this._mapper.Object,
            this._userManager.Object, this._messageBus.Object, this._serviceBusSettings.Object, this._dissertationApi.Object, this._httpContextAccessor.Object);

        #region TestData
        this._applicationUser = TestData.User;
        this._applicationUser.ProfilePicture = TestData.ProfilePicture;

        #endregion
    }

    [Test]
    public void GetPaginatedAdminUsers_ShouldReturnPaginatedUsersSuccessfully()
    {
        // Arrange
        var paginationParameters = new UserPaginationParameters();
        var users = new PagedList<ApplicationUser>(
            new List<ApplicationUser>
            {
                this._applicationUser!
            },
            count:1,
            pageNumber: paginationParameters.PageNumber,
            pageSize: paginationParameters.PageSize
        );

        var userDtos = users.Select(CustomMappers.MapToUserDto).ToList();
        var pagedUserDtos = new PagedList<UserListDto>(
            userDtos,
            users.TotalCount,
            users.CurrentPage,
            users.PageSize
        );

        this._mockUnitOfWork.Setup(db => db.ApplicationUserRepository.GetPaginatedAdminUsers(paginationParameters))
            .Returns(users);

        // Act
        ResponseDto<PaginatedUserListDto> result = this._userService.GetPaginatedAdminUsers(paginationParameters);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo(SuccessMessages.DefaultSuccess));
            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result!.TotalCount, Is.EqualTo(pagedUserDtos.TotalCount));
            Assert.That(result.Result.PageSize, Is.EqualTo(pagedUserDtos.PageSize));
            Assert.That(result.Result.CurrentPage, Is.EqualTo(pagedUserDtos.CurrentPage));
            Assert.That(result.Result.TotalPages, Is.EqualTo(pagedUserDtos.TotalPages));
            Assert.That(result.Result.HasNext, Is.EqualTo(pagedUserDtos.HasNext));
            Assert.That(result.Result.HasPrevious, Is.EqualTo(pagedUserDtos.HasPrevious));
        });
    }

    [Test]
    public void GetPaginatedStudents_ShouldThrowNotFoundException_WhenCoursesAreNotFound()
    {
        // Arrange
        var paginationParameters = new DissertationStudentPaginationParameters
        {
            PageNumber = 0,
            PageSize = 0,
            SearchByUserName = null!,
            FilterByCourse = 0,
            FilterByCohort = 0,
            CohortStartDate = default,
            CohortEndDate = default
        };
        this._dissertationApi.Setup(service => service.GetAllCourses())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetCourse>> { IsSuccess = false, Result = null });
        var users = new PagedList<ApplicationUser>(
            new List<ApplicationUser>
            {
                this._applicationUser!
            },
            count:1,
            pageNumber: paginationParameters.PageNumber,
            pageSize: paginationParameters.PageSize
        );

        this._mockUnitOfWork.Setup(db => db.ApplicationUserRepository.GetPaginatedStudents(paginationParameters))
            .Returns(users);

        // Act & Assert
         Assert.ThrowsAsync<NotFoundException>(async () => await this._userService.GetPaginatedStudents(paginationParameters));
    }

    [Test]
    public async Task GetPaginatedStudents_ShouldReturnPaginatedStudentsSuccessfully()
    {
        // Arrange
        var paginationParameters = new DissertationStudentPaginationParameters { /* ... populate test pagination parameters ... */ };
        var users = new PagedList<ApplicationUser>(
            new List<ApplicationUser> { /* ... populate test users ... */ },
            count:1,
            pageNumber: paginationParameters.PageNumber,
            pageSize: paginationParameters.PageSize
        );
        var courses = new ResponseDto<IReadOnlyList<GetCourse>> {
            IsSuccess = true,
            Result = new List<GetCourse> { /* ... populate test courses ... */ }
        };

        this._mockUnitOfWork.Setup(db => db.ApplicationUserRepository.GetPaginatedStudents(paginationParameters))
            .Returns(users);
        this._dissertationApi.Setup(service => service.GetAllCourses())
            .ReturnsAsync(courses);

        // Act
        ResponseDto<PaginatedStudentListDto> result = await this._userService.GetPaginatedStudents(paginationParameters);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo(SuccessMessages.DefaultSuccess));
            Assert.That(result.Result, Is.Not.Null);
        });
    }

    [Test]
    public void GetPaginatedSupervisors_ShouldThrowNotFoundException_WhenDepartmentsAreNotFound()
    {
        // Arrange
        var paginationParameters = new SupervisorPaginationParameters
        {
            PageNumber = 0,
            PageSize = 0,
            SearchByUserName = null!,
        };
        this._dissertationApi.Setup(service => service.GetAllDepartments())
            .ReturnsAsync(new ResponseDto<IReadOnlyList<GetDepartment>> { IsSuccess = false, Result = null });
        var users = new PagedList<ApplicationUser>(
            new List<ApplicationUser>
            {
                this._applicationUser!
            },
            count:1,
            pageNumber: paginationParameters.PageNumber,
            pageSize: paginationParameters.PageSize
        );

        this._mockUnitOfWork.Setup(db => db.ApplicationUserRepository.GetPaginatedSupervisors(paginationParameters))
            .Returns(users);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await this._userService.GetPaginatedSupervisors(paginationParameters));
    }

    [Test]
    public async Task GetPaginatedSupervisors_ShouldReturnPaginatedSupervisorsSuccessfully()
    {
        // Arrange
        var paginationParameters = new  SupervisorPaginationParameters{ /* ... populate test pagination parameters ... */ };
        var users = new PagedList<ApplicationUser>(
            new List<ApplicationUser> { /* ... populate test users ... */ },
            count:1,
            pageNumber: paginationParameters.PageNumber,
            pageSize: paginationParameters.PageSize
        );
        var departments = new ResponseDto<IReadOnlyList<GetDepartment>> {
            IsSuccess = true,
            Result = new List<GetDepartment> { /* ... populate test courses ... */ }
        };

        this._mockUnitOfWork.Setup(db => db.ApplicationUserRepository.GetPaginatedSupervisors(paginationParameters))
            .Returns(users);
        this._dissertationApi.Setup(service => service.GetAllDepartments())
            .ReturnsAsync(departments);

        // Act
        ResponseDto<PaginatedSupervisorListDto> result = await this._userService.GetPaginatedSupervisors(paginationParameters);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo(SuccessMessages.DefaultSuccess));
            Assert.That(result.Result, Is.Not.Null);
        });
    }
}