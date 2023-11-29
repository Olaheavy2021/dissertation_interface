using Dissertation.Application.AcademicYear.Queries.GetActiveAcademicYear;
using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using FluentAssertions;
using MapsterMapper;
using Moq;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;
using UnitTests.DissertationAPI.Mocks;

namespace UnitTests.DissertationAPI.Application.AcademicYear.Queries;

public class GetActiveAcademicYearQueryHandlerTest
{
    private readonly Mock<IAppLogger<GetActiveAcademicYearQueryHandler>> _logger = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly GetActiveAcademicYearQueryHandler _getActiveAcademicYearQueryHandler;

    public GetActiveAcademicYearQueryHandlerTest() =>
        this._getActiveAcademicYearQueryHandler = new GetActiveAcademicYearQueryHandler(
            this._logger.Object,
            this._unitOfWork.Object,
            new Mapper()
        );

    [Test]
    public async Task Returns_Active_AcademicYear()
    {
        GetActiveAcademicYearQuery query = new();
        this._unitOfWork
            .Setup(x => x.AcademicYearRepository.GetActiveAcademicYear())
            .ReturnsAsync(AcademicYearMocks.GetFirstOrDefaultResponse());

        ResponseDto<GetAcademicYear> response =
            await this._getActiveAcademicYearQueryHandler.Handle(query, this._cancellationToken);

        response.Should().NotBeNull();
        response.Result.Should().BeOfType<GetAcademicYear>();
        response.IsSuccess.Should().BeTrue();
    }

    [Test]
    public Task Returns_Inactive_AcademicYear()
    {
        GetActiveAcademicYearQuery query = new();
        this._unitOfWork
            .Setup(x => x.AcademicYearRepository.GetActiveAcademicYear())
            .ReturnsAsync((Dissertation.Domain.Entities.AcademicYear)null!);

        Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await this._getActiveAcademicYearQueryHandler.Handle(query, this._cancellationToken);
        });
        return Task.CompletedTask;
    }
}