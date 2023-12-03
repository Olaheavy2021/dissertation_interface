using Dissertation.Application.DissertationCohort.Queries.GetListOfDissertationCohort;
using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Enums;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Helpers;
using Shared.Logging;

namespace Dissertation.Application.SupervisorInvite.Queries.GetListOfSupervisorInvite;

public class GetSupervisorInviteListQueryHandler : IRequestHandler<GetSupervisorInviteListQuery, ResponseDto<PaginatedSupervisorInvite>>
{
    private readonly IAppLogger<GetSupervisorInviteListQueryHandler> _logger;
    private readonly IUnitOfWork _db;

    public GetSupervisorInviteListQueryHandler(IAppLogger<GetSupervisorInviteListQueryHandler> logger, IUnitOfWork db)
    {
        this._db = db;
        this._logger = logger;
    }


    public Task<ResponseDto<PaginatedSupervisorInvite>> Handle(GetSupervisorInviteListQuery request,
        CancellationToken cancellationToken)
    {
        var response = new ResponseDto<PaginatedSupervisorInvite>();
        this._logger.LogInformation("Attempting to retrieve list of Dissertation Cohort");
        PagedList<Domain.Entities.SupervisorInvite> supervisorInvites = this._db.SupervisorInviteRepository.GetListOfSupervisorInvites(request.Parameters);

        var mappedSupervisorInvite = new PagedList<GetSupervisorInvite>(
            supervisorInvites.Select(MapToSupervisorInviteDto).ToList(),
            supervisorInvites.TotalCount,
            supervisorInvites.CurrentPage,
            supervisorInvites.PageSize
        );

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = new PaginatedSupervisorInvite()
        {
            Data = mappedSupervisorInvite,
            TotalCount = mappedSupervisorInvite.TotalCount,
            PageSize = mappedSupervisorInvite.PageSize,
            CurrentPage = mappedSupervisorInvite.CurrentPage,
            TotalPages = mappedSupervisorInvite.TotalPages,
            HasNext = mappedSupervisorInvite.HasNext,
            HasPrevious = mappedSupervisorInvite.HasPrevious
        };

        return Task.FromResult(response);
    }

    private GetSupervisorInvite MapToSupervisorInviteDto(
        Domain.Entities.SupervisorInvite supervisorInvite) =>
        new()
        {
            Id = supervisorInvite.Id,
            FirstName = supervisorInvite.FirstName,
            Status = DateTime.UtcNow.Date > supervisorInvite.ExpiryDate.Date ? DissertationConfigStatus.Expired : DissertationConfigStatus.Active,
            StaffId = supervisorInvite.StaffId,
            LastName = supervisorInvite.LastName,
            ExpiryDate = supervisorInvite.ExpiryDate,
            Email = supervisorInvite.Email
        };
}