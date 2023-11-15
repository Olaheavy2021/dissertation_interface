using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Enums;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Logging;

namespace Dissertation.Application.DissertationCohort.Commands.EnableDissertationCohort;

public class EnableDissertationCohortCommandHandler : IRequestHandler<EnableDissertationCohortCommand, ResponseDto<GetDissertationCohort>>
{
    private readonly IAppLogger<EnableDissertationCohortCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public EnableDissertationCohortCommandHandler(IAppLogger<EnableDissertationCohortCommandHandler> logger, IUnitOfWork db, IMapper mapper)
    {
        this._logger = logger;
        this._db = db;
        this._mapper = mapper;
    }

    public async Task<ResponseDto<GetDissertationCohort>> Handle(EnableDissertationCohortCommand request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to Activate a Dissertation Cohort for this {id}", request.DissertationCohortId);
        var response = new ResponseDto<GetDissertationCohort>();

        //deactivate the active dissertation cohort
        Domain.Entities.DissertationCohort? activeDissertationCohort =
            await this._db.DissertationCohortRepository.GetActiveDissertationCohort(request.DissertationCohortId);

        if (activeDissertationCohort != null)
        {
            activeDissertationCohort.Status = DissertationConfigStatus.InActive;
            this._db.DissertationCohortRepository.Update(activeDissertationCohort);
        }

        Domain.Entities.DissertationCohort? dissertationCohort =
            await this._db.DissertationCohortRepository.GetAsync(x => x.Id == request.DissertationCohortId);
        GetDissertationCohort getDissertationCohort = new();

        if (dissertationCohort != null)
        {
            if (dissertationCohort.Status == DissertationConfigStatus.Active)
            {
                response.IsSuccess = false;
                response.Message = "Dissertation Cohort is already active";
                return response;
            }

            dissertationCohort.Status = DissertationConfigStatus.Active;
            this._db.DissertationCohortRepository.Update(dissertationCohort);
            GetDissertationCohort mappedDissertationCohort = this._mapper.Map<GetDissertationCohort>(dissertationCohort);
            await this._db.SaveAsync(cancellationToken);

            response.IsSuccess = true;
            response.Result = mappedDissertationCohort;
            response.Message = SuccessMessages.DefaultSuccess;

            return response;
        }

        response.IsSuccess = false;
        response.Message = ErrorMessages.DefaultError;
        return response;
    }
}