using System.Linq.Expressions;
using Dissertation.Application.AcademicYear.Queries.GetById;
using Dissertation.Application.DissertationCohort.Queries.GetById;
using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using FluentAssertions;
using MapsterMapper;
using Moq;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;
using UnitTests.DissertationAPI.Mocks;

namespace UnitTests.DissertationAPI.Application.DissertationCohort.Queries;

public class GetDissertationCohortByIdQueryHandlerTest
{
    private readonly Mock<IAppLogger<GetDissertationCohortByIdQueryHandler>> _logger = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly GetDissertationCohortByIdQueryHandler _getDissertationCohortByIdQueryHandler;

    public GetDissertationCohortByIdQueryHandlerTest() =>
        this._getDissertationCohortByIdQueryHandler = new GetDissertationCohortByIdQueryHandler(
            this._logger.Object,
            this._unitOfWork.Object,
            new Mapper()
        );

    [Test]
    public async Task Returns_DissertationCohort()
    {
        GetDissertationCohortByIdQuery query = new(24);
        this._unitOfWork
            .Setup(x => x.DissertationCohortRepository.GetAsync(
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.DissertationCohort, bool>>>(),
                It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.DissertationCohort>, IOrderedQueryable<Dissertation.Domain.Entities.DissertationCohort>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.DissertationCohort, object>>[]>()))
            .ReturnsAsync(DissertationCohortMocks.GetFirstOrDefaultResponse());

        ResponseDto<GetDissertationCohort> response =
            await this._getDissertationCohortByIdQueryHandler.Handle(query, this._cancellationToken);

        response.Should().NotBeNull();
        response.Result.Should().BeOfType<GetDissertationCohort>();
        response.IsSuccess.Should().BeTrue();
    }

    [Test]
    public Task Throws_NotFoundException()
    {
        GetDissertationCohortByIdQuery query = new(24);
        this._unitOfWork
            .Setup(x => x.DissertationCohortRepository.GetAsync(
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.DissertationCohort, bool>>>(),
                It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.DissertationCohort>, IOrderedQueryable<Dissertation.Domain.Entities.DissertationCohort>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.DissertationCohort, object>>[]>()))
            .ReturnsAsync((Dissertation.Domain.Entities.DissertationCohort)null!);

        Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await this._getDissertationCohortByIdQueryHandler.Handle(query, this._cancellationToken);
        });

        return Task.CompletedTask;

    }
}