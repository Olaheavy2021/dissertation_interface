using System.Linq.Expressions;
using Dissertation.Application.Course.Commands.UpdateCourse;
using Dissertation.Application.Department.Commands.UpdateDepartment;
using Dissertation.Application.DTO.Request;
using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using FluentAssertions;
using MapsterMapper;
using Moq;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;
using UnitTests.DissertationAPI.Mocks;

namespace UnitTests.DissertationAPI.Application.Course.Commands;

public class UpdateCourseCommandHandlerTest
{
    private readonly Mock<IAppLogger<UpdateCourseCommandHandler>> _logger = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly UpdateCourseCommandHandler _updateCourseCommandHandler;

    public UpdateCourseCommandHandlerTest() =>
        this._updateCourseCommandHandler = new UpdateCourseCommandHandler(
            this._logger.Object,
            this._unitOfWork.Object,
            new Mapper()
        );

    [Test]
    public async Task Update_Course_Successfully()
    {
        CreateCourseRequest request = CourseMocks.GetSuccessfulRequest();
        UpdateCourseCommand command = new(1, request.Name, 1);
        this._unitOfWork
            .Setup(x => x.CourseRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.Course, bool>>>(),
                It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.Course>, IOrderedQueryable<Dissertation.Domain.Entities.Course>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.Course, object>>[]>()))
            .ReturnsAsync(CourseMocks.GetFirstOrDefaultResponse);

        ResponseDto<GetCourse> response =
            await this._updateCourseCommandHandler.Handle(command, this._cancellationToken);

        response.Should().NotBeNull();
        response.Result.Should().BeOfType<GetCourse>();
        response.IsSuccess.Should().BeTrue();
    }

    [Test]
    public Task Update_Course_ThrowsNotFoundError()
    {
        CreateCourseRequest request = CourseMocks.GetSuccessfulRequest();
        UpdateCourseCommand command = new(1, request.Name, 1);
        this._unitOfWork
            .Setup(x => x.CourseRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.Course, bool>>>(),
                It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.Course>, IOrderedQueryable<Dissertation.Domain.Entities.Course>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.Course, object>>[]>()))
            .ReturnsAsync((Dissertation.Domain.Entities.Course)null!);

        Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await this._updateCourseCommandHandler.Handle(command, this._cancellationToken);
        });
        return Task.CompletedTask;
    }
}