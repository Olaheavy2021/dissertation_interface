using Dissertation.Application.DTO.Request;
using Dissertation.Application.Student.Queries.GetSupervisionRequests;
using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.DTO;
using Shared.Exceptions;

namespace Dissertation.Application.Supervisor.Queries.GetSupervisionRequests;

public class GetSupervisorSupervisionRequestsQueryHandler : IRequestHandler<GetSupervisorSupervisionRequestsQuery, ResponseDto<PaginatedSupervisionRequestListDto>>
{
    private readonly ILogger<GetSupervisorSupervisionRequestsQueryHandler> _logger;
    private readonly IUserApiService _userApiService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    public GetSupervisorSupervisionRequestsQueryHandler(ILogger<GetSupervisorSupervisionRequestsQueryHandler> logger, IUserApiService userApiService, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IMapper mapper)
    {
        this._logger = logger;
        this._userApiService = userApiService;
        this._unitOfWork = unitOfWork;
        this._httpContextAccessor = httpContextAccessor;
        this._mapper = mapper;
    }

    public async Task<ResponseDto<PaginatedSupervisionRequestListDto>> Handle(GetSupervisorSupervisionRequestsQuery request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Fetching Supervision Requests for a Supervisor");
        //fetch the supervisor from the database
        if (this._httpContextAccessor.HttpContext?.Items["UserId"] is not string userId)
        {
            this._logger.LogError("Invalid token passed to fetch list of supervision request");
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
                return new ResponseDto<PaginatedSupervisionRequestListDto>()
                {
                    IsSuccess = false,
                    Message = "Please filter by a dissertation cohort. There is no active cohort"
                };
            }

            request.Parameters.FilterByCohort = activeCohort.Id;
        }

        var apiRequest = new SupervisionRequestPaginationParameters()
        {
            SearchByStudent = request.Parameters.SearchByStudent,
            DissertationCohortId = request.Parameters.FilterByCohort,
            FilterByStatus = request.Parameters.FilterByStatus,
            PageNumber = request.Parameters.PageNumber,
            PageSize = request.Parameters.PageSize,
            SupervisorId = supervisor.UserId
        };

        ResponseDto<PaginatedSupervisionRequestListDto> response = await this._userApiService.GetSupervisionRequestsForASupervisor(apiRequest);
        return response;
    }
}