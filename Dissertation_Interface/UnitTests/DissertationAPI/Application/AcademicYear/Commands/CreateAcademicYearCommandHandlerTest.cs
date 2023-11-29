using System.Linq.Expressions;
using Dissertation.Application.AcademicYear.Commands.CreateAcademicYear;
using Dissertation.Application.DTO;
using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using FluentAssertions;
using MapsterMapper;
using Moq;
using Shared.DTO;
using Shared.Logging;
using UnitTests.DissertationAPI.Mocks;

namespace UnitTests.DissertationAPI.Application.AcademicYear.Commands;

public class CreateAcademicYearCommandHandlerTest
{
    private readonly Mock<IAppLogger<CreateAcademicYearCommandHandler>> _logger = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly CreateAcademicYearCommandHandler _createAcademicYearCommandHandler;

    public CreateAcademicYearCommandHandlerTest() =>
        this._createAcademicYearCommandHandler = new CreateAcademicYearCommandHandler(
            this._logger.Object,
            this._unitOfWork.Object,
            new Mapper()
        );

    [Test]
    public async Task Returns_Newly_Created_AcademicYear()
    {
        CreateAcademicYearRequest request = AcademicYearMocks.GetSuccessfulRequest();
        CreateAcademicYearCommand command = new(request.StartDate, request.EndDate);

        this._unitOfWork
            .Setup(x => x.AcademicYearRepository.AddAsync(It.IsAny<Dissertation.Domain.Entities.AcademicYear>()))
            .Returns(Task.CompletedTask);

        this._unitOfWork
            .Setup(x => x.AcademicYearRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Dissertation.Domain.Entities.AcademicYear, bool>>>(), null, null))
            .ReturnsAsync(AcademicYearMocks.GetFirstOrDefaultResponse());

        ResponseDto<GetAcademicYear> response =
            await this._createAcademicYearCommandHandler.Handle(command, this._cancellationToken);

        response.Should().NotBeNull();
        response.Result.Should().BeOfType<GetAcademicYear>();
        response.IsSuccess.Should().BeTrue();
    }
}