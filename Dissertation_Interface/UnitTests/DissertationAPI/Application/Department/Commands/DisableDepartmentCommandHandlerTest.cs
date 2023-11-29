using System.Linq.Expressions;
using Dissertation.Application.Department.Commands.DisableDepartment;
using Dissertation.Application.DTO.Request;
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

namespace UnitTests.DissertationAPI.Application.Department.Commands;

public class DisableDepartmentCommandHandlerTest
{
    private readonly Mock<IAppLogger<DisableDepartmentCommandHandler>> _logger = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly DisableDepartmentCommandHandler _disableDepartmentCommandHandler;

    public DisableDepartmentCommandHandlerTest() =>
        this._disableDepartmentCommandHandler = new DisableDepartmentCommandHandler(
            this._logger.Object,
            this._unitOfWork.Object,
            new Mapper()
        );

    [Test]
    public async Task Disables_Department_Successfully()
    {
        DisableDepartmentCommand command = new(1);
        this._unitOfWork
            .Setup(x => x.DepartmentRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.Department, bool>>>(),
                It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.Department>, IOrderedQueryable<Dissertation.Domain.Entities.Department>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.Department, object>>[]>()))
            .ReturnsAsync(DepartmentMock.GetFirstOrDefaultResponse());

        ResponseDto<GetDepartment> response =
            await this._disableDepartmentCommandHandler.Handle(command, this._cancellationToken);

        response.Should().NotBeNull();
        response.Result.Should().BeOfType<GetDepartment>();
        response.IsSuccess.Should().BeTrue();
    }

    [Test]
    public async Task Disables_Department_Failed()
    {
        DisableDepartmentCommand command = new(1);
        Dissertation.Domain.Entities.Department department = DepartmentMock.GetFirstOrDefaultResponse();
        department.Status = DissertationConfigStatus.InActive;
        this._unitOfWork
            .Setup(x => x.DepartmentRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.Department, bool>>>(),
                It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.Department>, IOrderedQueryable<Dissertation.Domain.Entities.Department>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.Department, object>>[]>()))
            .ReturnsAsync(department);

        ResponseDto<GetDepartment> response =
            await this._disableDepartmentCommandHandler.Handle(command, this._cancellationToken);

        response.Should().NotBeNull();
        response.Result.Should().BeNull();
        response.IsSuccess.Should().BeFalse();
    }

    [Test]
    public Task DisableDepartment_ThrowsNotFoundException()
    {
        DisableDepartmentCommand command = new(1);
        this._unitOfWork
            .Setup(x => x.DepartmentRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.Department, bool>>>(),
                It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.Department>, IOrderedQueryable<Dissertation.Domain.Entities.Department>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.Department, object>>[]>()))
            .ReturnsAsync((Dissertation.Domain.Entities.Department)null!);

        Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await this._disableDepartmentCommandHandler.Handle(command, this._cancellationToken);
        });
        return Task.CompletedTask;
    }
}