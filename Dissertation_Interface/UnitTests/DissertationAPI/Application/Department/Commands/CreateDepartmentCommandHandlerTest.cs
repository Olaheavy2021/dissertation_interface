using Dissertation.Application.AcademicYear.Commands.CreateAcademicYear;
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

namespace UnitTests.DissertationAPI.Application.Department.Commands;

public class CreateDepartmentCommandHandlerTest
{
    private readonly Mock<IAppLogger<CreateDepartmentCommandHandler>> _logger = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly CreateDepartmentCommandHandler _createDepartmentCommandHandler;

    public CreateDepartmentCommandHandlerTest() =>
        this._createDepartmentCommandHandler = new CreateDepartmentCommandHandler(
            this._logger.Object,
            this._unitOfWork.Object,
            new Mapper()
        );

    [Test]
    public async Task Returns_Newly_Created_Department()
    {
        CreateDepartmentRequest request = DepartmentMock.GetSuccessfulRequest();
        CreateDepartmentCommand command = new(request.Name);

        this._unitOfWork
            .Setup(x => x.DepartmentRepository.AddAsync(It.IsAny<Dissertation.Domain.Entities.Department>()))
            .Returns(Task.CompletedTask);

        ResponseDto<GetDepartment> response =
            await this._createDepartmentCommandHandler.Handle(command, this._cancellationToken);

        response.Should().NotBeNull();
        response.Result.Should().BeOfType<GetDepartment>();
        response.IsSuccess.Should().BeTrue();
    }
}