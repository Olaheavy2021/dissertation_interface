using Dissertation.Application.AcademicYear.Queries.GetListOfAcademicYear;
using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Pagination;
using Dissertation.Infrastructure.Persistence.IRepository;
using FluentAssertions;
using Moq;
using Shared.DTO;
using Shared.Helpers;
using Shared.Logging;
using UnitTests.DissertationAPI.Mocks;

namespace UnitTests.DissertationAPI.Application.AcademicYear.Queries;

public class GetListOfAcademicYearQueryHandlerTest
{
    private readonly Mock<IAppLogger<GetListOfAcademicYearQueryHandler>> _logger = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly GetListOfAcademicYearQueryHandler _getListOfAcademicYearQueryHandler;

    public GetListOfAcademicYearQueryHandlerTest() =>
        this._getListOfAcademicYearQueryHandler = new GetListOfAcademicYearQueryHandler(
            this._logger.Object,
            this._unitOfWork.Object
        );

    [Test]
    public async Task Returns_ListOf_AcademicYear()
    {
        var parameters = new AcademicYearPaginationParameters()
        {
            PageSize = 10, PageNumber = 1, SearchByYear = 2023, FilterByStatus = "Active"
        };
        GetListOfAcademicYearQuery query = new(parameters);
        this._unitOfWork
            .Setup(x => x.AcademicYearRepository.GetListOfAcademicYears(It.IsAny<AcademicYearPaginationParameters>()))
            .Returns(AcademicYearMocks.GetPaginatedResponse());

        ResponseDto<PaginatedAcademicYearListDto> response =
            await this._getListOfAcademicYearQueryHandler.Handle(query, this._cancellationToken);

        response.Should().NotBeNull();
        response.Result.Should().BeOfType<PaginatedAcademicYearListDto>();
        response.IsSuccess.Should().BeTrue();
    }
}

