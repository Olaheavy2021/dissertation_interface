using Dissertation.Application.Course.Commands.CreateCourse;
using Dissertation.Application.Department.Commands.CreateDepartment;
using Dissertation.Application.DTO.Request;
using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using FluentAssertions;
using MapsterMapper;
using Moq;
using Shared.DTO;
using Shared.Logging;
using UnitTests.DissertationAPI.Mocks;

namespace UnitTests.DissertationAPI.Application.Course.Commands;

public class CreateCourseCommandHandlerTest
{
    private readonly Mock<IAppLogger<CreateCourseCommandHandler>> _logger = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly CreateCourseCommandHandler _createCourseCommandHandler;

    public CreateCourseCommandHandlerTest() =>
        this._createCourseCommandHandler = new CreateCourseCommandHandler(
            this._logger.Object,
            this._unitOfWork.Object,
            new Mapper()
        );

    [Test]
    public async Task Returns_Newly_Created_Department()
    {
        CreateCourseRequest request = CourseMocks.GetSuccessfulRequest();
        CreateCourseCommand command = new(request.Name, request.DepartmentId);

        this._unitOfWork
            .Setup(x => x.CourseRepository.AddAsync(It.IsAny<Dissertation.Domain.Entities.Course>()))
            .Returns(Task.CompletedTask);

        ResponseDto<GetCourse> response =
            await this._createCourseCommandHandler.Handle(command, this._cancellationToken);

        response.Should().NotBeNull();
        response.Result.Should().BeOfType<GetCourse>();
        response.IsSuccess.Should().BeTrue();
    }
}