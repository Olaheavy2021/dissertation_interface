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

namespace UnitTests.DissertationAPI.Application.Department.Commands;

public class UpdateDepartmentCommandHandlerTest
{
    private readonly Mock<IAppLogger<UpdateDepartmentCommandHandler>> _logger = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly UpdateDepartmentCommandHandler _updateDepartmentCommandHandler;

    public UpdateDepartmentCommandHandlerTest() =>
        this._updateDepartmentCommandHandler = new UpdateDepartmentCommandHandler(
            this._logger.Object,
            this._unitOfWork.Object,
            new Mapper()
        );

    [Test]
    public async Task Update_Department_Successfully()
    {
        CreateDepartmentRequest request = DepartmentMock.GetSuccessfulRequest();
        UpdateDepartmentCommand command = new(request.Name, 1);
        this._unitOfWork
            .Setup(x => x.DepartmentRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.Department, bool>>>(),
                It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.Department>, IOrderedQueryable<Dissertation.Domain.Entities.Department>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.Department, object>>[]>()))
            .ReturnsAsync(DepartmentMock.GetFirstOrDefaultResponse());

        ResponseDto<GetDepartment> response =
            await this._updateDepartmentCommandHandler.Handle(command, this._cancellationToken);

        response.Should().NotBeNull();
        response.Result.Should().BeOfType<GetDepartment>();
        response.IsSuccess.Should().BeTrue();
    }

    [Test]
    public Task Update_Department_ThrowsNotFoundError()
    {
        CreateDepartmentRequest request = DepartmentMock.GetSuccessfulRequest();
        UpdateDepartmentCommand command = new(request.Name, 1);
        this._unitOfWork
            .Setup(x => x.DepartmentRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.Department, bool>>>(),
                It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.Department>, IOrderedQueryable<Dissertation.Domain.Entities.Department>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.Department, object>>[]>()))
            .ReturnsAsync((Dissertation.Domain.Entities.Department)null!);

        Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await this._updateDepartmentCommandHandler.Handle(command, this._cancellationToken);
        });
        return Task.CompletedTask;
    }
}