using Dissertation.Application.AcademicYear.Commands.UpdateAcademicYear;
using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.DissertationCohort.Commands.UpdateDissertationCohort;

public class UpdateDissertationCohortCommandHandler : IRequestHandler<UpdateDissertationCohortCommand, ResponseDto<GetDissertationCohort>>
{
    private readonly IAppLogger<UpdateDissertationCohortCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public UpdateDissertationCohortCommandHandler(IAppLogger<UpdateDissertationCohortCommandHandler> logger, IUnitOfWork db, IMapper mapper)
    {
        this._logger = logger;
        this._db = db;
        this._mapper = mapper;
    }

    public async Task<ResponseDto<GetDissertationCohort>> Handle(UpdateDissertationCohortCommand request,
        CancellationToken cancellationToken)
    {
        var response = new ResponseDto<GetDissertationCohort>();
        //fetch the academic year from the database
        Domain.Entities.DissertationCohort? dissertationCohort = await this._db.DissertationCohortRepository.GetFirstOrDefaultAsync(a => a.Id == request.Id);
        if (dissertationCohort == null)
        {
            this._logger.LogError("No Dissertation Cohort found with ID");
            throw new NotFoundException(nameof(DissertationCohort), request.Id);
        }

        //update the database
        dissertationCohort.StartDate = request.StartDate;
        dissertationCohort.EndDate = request.EndDate;
        dissertationCohort.SupervisionChoiceDeadline = request.SupervisionChoiceDeadline;
        this._db.DissertationCohortRepository.Update(dissertationCohort);
        await this._db.SaveAsync(cancellationToken);

        GetDissertationCohort mappedDissertationCohort = this._mapper.Map<GetDissertationCohort>(dissertationCohort);
        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedDissertationCohort;
        return response;
    }
}