using Dissertation.Application.DissertationCohort.Queries.GetActiveDissertationCohort;
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

public class GetActiveDissertationCohortQueryHandlerTest
{
    private readonly Mock<IAppLogger<GetActiveDissertationCohortQueryHandler>> _logger = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly GetActiveDissertationCohortQueryHandler _getActiveDissertationCohortQueryHandler;

    public GetActiveDissertationCohortQueryHandlerTest() =>
        this._getActiveDissertationCohortQueryHandler = new GetActiveDissertationCohortQueryHandler(
            this._logger.Object,
            this._unitOfWork.Object,
            new Mapper()
        );

    [Test]
    public async Task Returns_Active_DissertationCohort()
    {
        GetActiveDissertationCohortQuery query = new();
        this._unitOfWork
            .Setup(x => x.DissertationCohortRepository.GetActiveDissertationCohort())
            .ReturnsAsync(DissertationCohortMocks.GetFirstOrDefaultResponse());

        ResponseDto<GetDissertationCohort> response =
            await this._getActiveDissertationCohortQueryHandler.Handle(query, this._cancellationToken);

        response.Should().NotBeNull();
        response.Result.Should().BeOfType<GetDissertationCohort>();
        response.IsSuccess.Should().BeTrue();
    }

    [Test]
    public Task Throws_NotFoundException()
    {
        GetActiveDissertationCohortQuery query = new();
        this._unitOfWork
            .Setup(x => x.DissertationCohortRepository.GetActiveDissertationCohort())
            .ReturnsAsync((Dissertation.Domain.Entities.DissertationCohort)null!);

        Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await this._getActiveDissertationCohortQueryHandler.Handle(query, this._cancellationToken);
        });
        return Task.CompletedTask;
    }
}