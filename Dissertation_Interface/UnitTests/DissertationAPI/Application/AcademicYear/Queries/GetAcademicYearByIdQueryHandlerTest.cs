using System.Linq.Expressions;
using Dissertation.Application.AcademicYear.Queries.GetById;
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

public class GetAcademicYearByIdQueryHandlerTest
{
    private readonly Mock<IAppLogger<GetAcademicYearByIdQueryHandler>> _logger = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly GetAcademicYearByIdQueryHandler _getAcademicYearByIdQueryHandler;

    public GetAcademicYearByIdQueryHandlerTest() =>
        this._getAcademicYearByIdQueryHandler = new GetAcademicYearByIdQueryHandler(
            this._logger.Object,
            this._unitOfWork.Object,
            new Mapper()
        );

    [Test]
    public async Task Returns_AcademicYear()
    {
        GetAcademicYearByIdQuery query = new(24);
        this._unitOfWork
            .Setup(x => x.AcademicYearRepository.GetAsync(
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.AcademicYear, bool>>>(),
                It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.AcademicYear>, IOrderedQueryable<Dissertation.Domain.Entities.AcademicYear>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.AcademicYear, object>>[]>()))
            .ReturnsAsync(AcademicYearMocks.GetFirstOrDefaultResponse());

        ResponseDto<GetAcademicYear> response =
            await this._getAcademicYearByIdQueryHandler.Handle(query, this._cancellationToken);

        response.Should().NotBeNull();
        response.Result.Should().BeOfType<GetAcademicYear>();
        response.IsSuccess.Should().BeTrue();
    }

    [Test]
    public Task Throws_NotFoundException()
    {
        GetAcademicYearByIdQuery query = new(24);
        this._unitOfWork
            .Setup(x => x.AcademicYearRepository.GetAsync(
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.AcademicYear, bool>>>(),
                It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.AcademicYear>, IOrderedQueryable<Dissertation.Domain.Entities.AcademicYear>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.AcademicYear, object>>[]>()))
            .ReturnsAsync((Dissertation.Domain.Entities.AcademicYear)null!);

        Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await this._getAcademicYearByIdQueryHandler.Handle(query, this._cancellationToken);
        });

        return Task.CompletedTask;

    }
}