using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.DTO;
using Shared.Exceptions;

namespace Dissertation.Application.Supervisor.Queries.GetSupervisionLists;

public class GetSupervisorSupervisionListQueryHandler : IRequestHandler<GetSupervisorSupervisionListQuery, ResponseDto<PaginatedSupervisionListDto>>
{
    private readonly ILogger<GetSupervisorSupervisionListQueryHandler> _logger;
    private readonly IUserApiService _userApiService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetSupervisorSupervisionListQueryHandler(ILogger<GetSupervisorSupervisionListQueryHandler> logger, IUserApiService userApiService, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        this._logger = logger;
        this._userApiService = userApiService;
        this._unitOfWork = unitOfWork;
        this._httpContextAccessor = httpContextAccessor;
    }

    public async Task<ResponseDto<PaginatedSupervisionListDto>> Handle(GetSupervisorSupervisionListQuery request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Fetching Supervision Lists for a Supervisor");
        //fetch the supervisor from the database
        var userId = this._httpContextAccessor.HttpContext?.Items["UserId"] as string;
        if (userId == null)
        {
            this._logger.LogError("Invalid token passed to fetch list of supervision lists");
            throw new NotFoundException("HttpContext", "UserId");
        }
        Domain.Entities.Supervisor? supervisor = await this._unitOfWork.SupervisorRepository.GetFirstOrDefaultAsync(a => a.UserId == userId);
        if (supervisor == null)
        {
            this._logger.LogError($"No Supervisor found with {userId}", userId);
            throw new NotFoundException(nameof(Domain.Entities.Student), userId);
        }

        if (request.Parameters.FilterByCohort == 0)
        {
            //fetch the active cohort
          Domain.Entities.DissertationCohort? activeCohort = await this._unitOfWork.DissertationCohortRepository.GetActiveDissertationCohort();
          if (activeCohort == null)
          {
              return new ResponseDto<PaginatedSupervisionListDto>()
              {
                  IsSuccess = false, Message = "Please filter by a dissertation cohort. There is no active cohort"
              };
          }

          request.Parameters.FilterByCohort = activeCohort.Id;
        }

        var apiRequest = new SupervisionListPaginationParameters()
        {
            SearchByStudent = request.Parameters.SearchByStudent,
            DissertationCohortId = request.Parameters.FilterByCohort,
            PageNumber = request.Parameters.PageNumber,
            PageSize = request.Parameters.PageSize,
            SupervisorId = supervisor.UserId
        };

        ResponseDto<PaginatedSupervisionListDto> response = await this._userApiService.GetSupervisionListForASupervisor(apiRequest);
        return response;
    }
}