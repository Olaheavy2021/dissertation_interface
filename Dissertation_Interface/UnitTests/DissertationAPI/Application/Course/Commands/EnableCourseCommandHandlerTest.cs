using System.Linq.Expressions;
using Dissertation.Application.Course.Commands.EnableCourse;
using Dissertation.Application.Department.Commands.EnableDepartment;
using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Enums;
using Dissertation.Infrastructure.Persistence.IRepository;
using FluentAssertions;
using MapsterMapper;
using Moq;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;
using UnitTests.DissertationAPI.Mocks;

namespace UnitTests.DissertationAPI.Application.Course.Commands;

public class EnableCourseCommandHandlerTest
{
    private readonly Mock<IAppLogger<EnableCourseCommandHandler>> _logger = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly EnableCourseCommandHandler _enableCourseCommandHandler;

    public EnableCourseCommandHandlerTest() =>
        this._enableCourseCommandHandler = new EnableCourseCommandHandler(
            this._logger.Object,
            this._unitOfWork.Object,
            new Mapper()
        );

    [Test]
    public async Task Enable_Course_Successfully()
    {
        EnableCourseCommand command = new(1);
        Dissertation.Domain.Entities.Course course = CourseMocks.GetFirstOrDefaultResponse();
        course.Status = DissertationConfigStatus.InActive;
        this._unitOfWork
            .Setup(x => x.CourseRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.Course, bool>>>(),
                It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.Course>, IOrderedQueryable<Dissertation.Domain.Entities.Course>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.Course, object>>[]>()))
            .ReturnsAsync(course);

        ResponseDto<GetCourse> response =
            await this._enableCourseCommandHandler.Handle(command, this._cancellationToken);

        response.Should().NotBeNull();
        response.Result.Should().BeOfType<GetCourse>();
        response.IsSuccess.Should().BeTrue();
    }

    [Test]
    public async Task Enable_Course_Failed()
    {
        EnableCourseCommand command = new(1);
        Dissertation.Domain.Entities.Course course = CourseMocks.GetFirstOrDefaultResponse();
        course.Status = DissertationConfigStatus.Active;
        this._unitOfWork
            .Setup(x => x.CourseRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.Course, bool>>>(),
                It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.Course>, IOrderedQueryable<Dissertation.Domain.Entities.Course>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.Course, object>>[]>()))
            .ReturnsAsync(course);

        ResponseDto<GetCourse> response =
            await this._enableCourseCommandHandler.Handle(command, this._cancellationToken);

        response.Should().NotBeNull();
        response.Result.Should().BeNull();
        response.IsSuccess.Should().BeFalse();
    }

    [Test]
    public Task EnableCourse_ThrowsNotFoundException()
    {
        EnableCourseCommand command = new(1);
        this._unitOfWork
            .Setup(x => x.CourseRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.Course, bool>>>(),
                It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.Course>, IOrderedQueryable<Dissertation.Domain.Entities.Course>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.Course, object>>[]>()))
            .ReturnsAsync((Dissertation.Domain.Entities.Course)null!);

        Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await this._enableCourseCommandHandler.Handle(command, this._cancellationToken);
        });
        return Task.CompletedTask;
    }
}