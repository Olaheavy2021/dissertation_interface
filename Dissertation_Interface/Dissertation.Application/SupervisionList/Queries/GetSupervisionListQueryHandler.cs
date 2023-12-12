using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.DTO;
using Shared.Exceptions;

namespace Dissertation.Application.SupervisionList.Queries;

public class GetSupervisionListQueryHandler : IRequestHandler<GetSupervisionListQuery, ResponseDto<PaginatedSupervisionListDto>>
{
    private readonly ILogger<GetSupervisionListQueryHandler> _logger;
    private readonly IUserApiService _userApiService;
    private readonly IUnitOfWork _unitOfWork;
    public GetSupervisionListQueryHandler(ILogger<GetSupervisionListQueryHandler> logger, IUserApiService userApiService, IUnitOfWork unitOfWork)
    {
        this._logger = logger;
        this._userApiService = userApiService;
        this._unitOfWork = unitOfWork;
    }

    public async Task<ResponseDto<PaginatedSupervisionListDto>> Handle(GetSupervisionListQuery request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Fetching List of Supervision Lists");
        if (request.Parameters.DissertationCohortId == 0)
        {
            Domain.Entities.DissertationCohort? activeCohort = await this._unitOfWork.DissertationCohortRepository.GetActiveDissertationCohort();
            if (activeCohort == null)
            {
                throw new NotFoundException(nameof(DissertationCohort), "Active");
            }

            request.Parameters.DissertationCohortId = activeCohort.Id;
        }

        var apiRequest = new SupervisionListPaginationParameters()
        {
            SearchBySupervisor = request.Parameters.SearchBySupervisor,
            DissertationCohortId = request.Parameters.DissertationCohortId,
            PageNumber = request.Parameters.PageNumber,
            PageSize = request.Parameters.PageSize
        };

        ResponseDto<PaginatedSupervisionListDto> response = await this._userApiService.GetSupervisionLists(apiRequest);
        return response;
    }
}