using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.SupervisionCohort.Queries.GetById;

public class GetAssignedSupervisorByIdQueryHandler : IRequestHandler<GetAssignedSupervisorByIdQuery, ResponseDto<GetSupervisionCohortDetails>>
{
    private readonly IAppLogger<GetAssignedSupervisorByIdQueryHandler> _logger;
    private readonly IUserApiService _userApiService;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public GetAssignedSupervisorByIdQueryHandler(IAppLogger<GetAssignedSupervisorByIdQueryHandler> logger, IUserApiService userApiService, IUnitOfWork db, IMapper mapper)
    {
        this._logger = logger;
        this._userApiService = userApiService;
        this._db = db;
        this._mapper = mapper;
    }

    public async Task<ResponseDto<GetSupervisionCohortDetails>> Handle(GetAssignedSupervisorByIdQuery request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to retrieve an assigned supervisor");
       ResponseDto<GetSupervisionCohort> userDetails = await this._userApiService.GetSupervisionCohort(request.Id);

       if (!userDetails.IsSuccess && userDetails.Result == null && userDetails.Result.UserDetails.Id == null)
       {
           return new ResponseDto<GetSupervisionCohortDetails>()
           {
               Message = "Supervision Cohort not found", IsSuccess = false
           };
       }

       //fetch the supervisor details
       Domain.Entities.Supervisor? supervisor =
           await this._db.SupervisorRepository.GetFirstOrDefaultAsync(
               x => x.UserId == userDetails.Result.UserDetails.Id, includes: x =>x.Department);

       if (supervisor == null)
       {
           throw new NotFoundException(nameof(Domain.Entities.Supervisor), userDetails.Result?.UserDetails?.Id);
       }

       SupervisorDto mappedSupervisor = this._mapper.Map<SupervisorDto>(supervisor);
       return new ResponseDto<GetSupervisionCohortDetails>()
       {
           Message = SuccessMessages.DefaultSuccess, IsSuccess = true, Result = new GetSupervisionCohortDetails()
           {
               Id = userDetails.Result.Id,
               UserDetails = userDetails.Result?.UserDetails,
               SupervisorDetails = mappedSupervisor,
               SupervisionSlot = userDetails.Result.SupervisionSlot,
               DissertationCohortId = userDetails.Result.DissertationCohortId
           }
       };
    }
}