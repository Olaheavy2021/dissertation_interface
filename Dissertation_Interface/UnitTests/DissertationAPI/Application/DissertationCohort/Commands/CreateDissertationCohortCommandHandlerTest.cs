using System.Linq.Expressions;
using Dissertation.Application.DissertationCohort.Commands.CreateDissertationCohort;
using Dissertation.Application.DissertationCohort.Commands.UpdateDissertationCohort;
using Dissertation.Application.DTO.Request;
using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using FluentAssertions;
using MapsterMapper;
using Moq;
using Shared.DTO;
using Shared.Logging;
using UnitTests.DissertationAPI.Mocks;

namespace UnitTests.DissertationAPI.Application.DissertationCohort.Commands;

public class CreateDissertationCohortCommandHandlerTest
{
    private readonly Mock<IAppLogger<CreateDissertationCohortCommandHandler>> _logger = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly CreateDissertationCohortCommandHandler _createDissertationCohortCommandHandler;

    public CreateDissertationCohortCommandHandlerTest() =>
        this._createDissertationCohortCommandHandler = new CreateDissertationCohortCommandHandler(
            this._logger.Object,
            this._unitOfWork.Object,
            new Mapper()
        );

    [Test]
    public async Task Returns_Newly_Created_AcademicYear()
    {
        this._unitOfWork
            .Setup(x => x.DissertationCohortRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Dissertation.Domain.Entities.DissertationCohort, bool>>>(),  It.IsAny<Func<IQueryable<Dissertation.Domain.Entities.DissertationCohort>, IOrderedQueryable<Dissertation.Domain.Entities.DissertationCohort>>>(),
                It.IsAny<Expression<Func<Dissertation.Domain.Entities.DissertationCohort, object>>[]>()))
            .ReturnsAsync(DissertationCohortMocks.GetFirstOrDefaultResponse());

        Dissertation.Domain.Entities.AcademicYear academicYear = AcademicYearMocks.GetFirstOrDefaultResponse();
        CreateDissertationCohortRequest request = DissertationCohortMocks.GetSuccessfulRequest(academicYear.StartDate, academicYear.EndDate);
        CreateDissertationCohortCommand command = new(request.StartDate, request.EndDate, request.SupervisionChoiceDeadline, request.AcademicYearId);


        ResponseDto<GetDissertationCohort> response =
            await this._createDissertationCohortCommandHandler.Handle(command, this._cancellationToken);

        response.Should().NotBeNull();
        response.Result.Should().BeOfType<GetDissertationCohort>();
        response.IsSuccess.Should().BeTrue();
    }
}