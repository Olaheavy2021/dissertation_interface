using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.DTO;
using Shared.Logging;

namespace Dissertation.Application.DissertationCohort.Commands.CreateDissertationCohort;

public class CreateDissertationCohortCommandHandler : IRequestHandler<CreateDissertationCohortCommand, ResponseDto<GetDissertationCohort>>
{
    private readonly IAppLogger<CreateDissertationCohortCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public CreateDissertationCohortCommandHandler(IAppLogger<CreateDissertationCohortCommandHandler> logger, IUnitOfWork db, IMapper mapper)
    {
        this._logger = logger;
        this._db = db;
        this._mapper = mapper;
    }

    public async Task<ResponseDto<GetDissertationCohort>> Handle(CreateDissertationCohortCommand request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to Create Dissertation Cohort for this {startDate}", request.StartDate);
        var response = new ResponseDto<GetDissertationCohort>();
        var dissertationCohort = Domain.Entities.DissertationCohort.Create(
            request.EndDate, request.StartDate, request.SupervisionChoiceDeadline, request.AcademicYearId
        );

        await this._db.DissertationCohortRepository.AddAsync(dissertationCohort);

        await this._db.SaveAsync(cancellationToken);

        GetDissertationCohort mappedDissertationCohort = this._mapper.Map<GetDissertationCohort>(dissertationCohort);
        mappedDissertationCohort.UpdateStatus();

        response.Message = "Dissertation Cohort initiated successfully";
        response.Result = mappedDissertationCohort;
        response.IsSuccess = true;
        this._logger.LogInformation("Dissertation Cohort created for this {startDate}", request.StartDate);
        return response;
    }
}