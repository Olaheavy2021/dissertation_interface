using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Constants;
using Shared.DTO;

namespace Dissertation.Application.DissertationCohort.Queries.GetAdminMetrics;

public class GetAdminMetricsQueryHandler : IRequestHandler<GetAdminMetricsQuery, ResponseDto<AdminMetricsResponse>>
{
    private readonly IUnitOfWork _db;
    private readonly ILogger<GetAdminMetricsQueryHandler> _logger;
    private readonly IUserApiService _userApiService;

    public GetAdminMetricsQueryHandler(IUnitOfWork db, ILogger<GetAdminMetricsQueryHandler> logger, IUserApiService userApiService)
    {
        this._db = db;
        this._logger = logger;
        this._userApiService = userApiService;
    }
    public async Task<ResponseDto<AdminMetricsResponse>> Handle(GetAdminMetricsQuery request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Fetching the metrics for the active cohort");
        Domain.Entities.DissertationCohort? activeCohort = await this._db.DissertationCohortRepository.GetActiveDissertationCohort();
        if (activeCohort == null)
        {
            return new ResponseDto<AdminMetricsResponse>()
            {
                Message = "There is no active cohort at the moment", IsSuccess = false
            };
        }

        //get the student count for the cohort
        var studentCount = await this._db.StudentRepository.CountWhere(x => x.DissertationCohortId == activeCohort.Id);

        //get the supervisor, and supervision request from the user API
        ResponseDto<SupervisionCohortMetricsDto> userApiResponse = await this._userApiService.GetSupervisionCohortMetrics(activeCohort.Id);
        if (userApiResponse.IsSuccess && userApiResponse.Result != null)
        {
            return new ResponseDto<AdminMetricsResponse>()
            {
                IsSuccess = true,
                Message = SuccessMessages.DefaultSuccess,
                Result = new AdminMetricsResponse()
                {
                    ApprovedRequests = userApiResponse.Result.ApprovedRequests,
                    DeclinedRequests = userApiResponse.Result.DeclinedRequests,
                    Supervisors = userApiResponse.Result.Supervisors,
                    Students = studentCount
                }
            };
        }

        return new ResponseDto<AdminMetricsResponse>() { IsSuccess = false, Message = userApiResponse.Message };
    }
}