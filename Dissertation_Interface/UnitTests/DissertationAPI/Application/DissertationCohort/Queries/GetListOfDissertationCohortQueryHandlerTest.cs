using Dissertation.Application.AcademicYear.Queries.GetListOfAcademicYear;
using Dissertation.Application.Department.Queries.GetListOfDepartment;
using Dissertation.Application.DissertationCohort.Queries.GetActiveDissertationCohort;
using Dissertation.Application.DissertationCohort.Queries.GetListOfDissertationCohort;
using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Pagination;
using Dissertation.Infrastructure.Persistence.IRepository;
using FluentAssertions;
using MapsterMapper;
using Moq;
using Shared.DTO;
using Shared.Helpers;
using Shared.Logging;
using UnitTests.DissertationAPI.Mocks;

namespace UnitTests.DissertationAPI.Application.DissertationCohort.Queries;

public class GetListOfDissertationCohortQueryHandlerTest
{
    private readonly Mock<IAppLogger<GetListOfDissertationCohortQueryHandler>> _logger = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly GetListOfDissertationCohortQueryHandler _getListOfDissertationCohortQueryHandler;

    public GetListOfDissertationCohortQueryHandlerTest() =>
        this._getListOfDissertationCohortQueryHandler = new GetListOfDissertationCohortQueryHandler(
            this._logger.Object,
            this._unitOfWork.Object,
            new Mapper()
        );

    [Test]
    public async Task Returns_ListOf_DissertationCohort()
    {
        var parameters = new DissertationCohortPaginationParameters()
        {
            PageSize = 10, PageNumber = 1, SearchByStartYear = 2022, FilterByStatus = "Active"
        };
        GetListOfDissertationCohortQuery query = new(parameters);
        this._unitOfWork
            .Setup(x => x.DissertationCohortRepository.GetListOfDissertationCohort(It.IsAny<DissertationCohortPaginationParameters>()))
            .Returns(DissertationCohortMocks.GetPaginatedResponse());

        ResponseDto<PaginatedDissertationCohortListDto> response =
            await this._getListOfDissertationCohortQueryHandler.Handle(query, this._cancellationToken);

        response.Should().NotBeNull();
        response.Result.Should().BeOfType<PaginatedDissertationCohortListDto>();
        response.IsSuccess.Should().BeTrue();
    }
}

