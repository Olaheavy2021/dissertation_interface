using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.DTO;
using Shared.Exceptions;

namespace Dissertation.Application.SupervisionRequest.Queries;

public class GetSupervisionRequestListQueryHandler : IRequestHandler<GetSupervisionRequestListQuery, ResponseDto<PaginatedSupervisionRequestListDto>>
{
    private readonly ILogger<GetSupervisionRequestListQueryHandler> _logger;
    private readonly IUserApiService _userApiService;
    private readonly IUnitOfWork _unitOfWork;
    public GetSupervisionRequestListQueryHandler(ILogger<GetSupervisionRequestListQueryHandler> logger, IUserApiService userApiService, IUnitOfWork unitOfWork)
    {
        this._logger = logger;
        this._userApiService = userApiService;
        this._unitOfWork = unitOfWork;
    }

    public async Task<ResponseDto<PaginatedSupervisionRequestListDto>> Handle(GetSupervisionRequestListQuery request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Fetching List of Supervision Request");
        if (request.Parameters.DissertationCohortId == 0)
        {
            Domain.Entities.DissertationCohort? activeCohort = await this._unitOfWork.DissertationCohortRepository.GetActiveDissertationCohort() ?? throw new NotFoundException(nameof(DissertationCohort), "Active");
            request.Parameters.DissertationCohortId = activeCohort.Id;
        }

        var apiRequest = new SupervisionRequestPaginationParameters()
        {
            SearchBySupervisor = request.Parameters.SearchBySupervisor,
            DissertationCohortId = request.Parameters.DissertationCohortId,
            FilterByStatus = request.Parameters.FilterByStatus,
            PageNumber = request.Parameters.PageNumber,
            PageSize = request.Parameters.PageSize
        };

        ResponseDto<PaginatedSupervisionRequestListDto> response = await this._userApiService.GetSupervisionRequests(apiRequest);
        return response;
    }
}