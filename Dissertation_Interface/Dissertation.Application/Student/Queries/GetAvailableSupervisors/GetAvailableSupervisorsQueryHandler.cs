using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.Student.Queries.GetAvailableSupervisors;

public class GetAvailableSupervisorsQueryHandler : IRequestHandler<GetAvailableSupervisorsQuery, ResponseDto<PaginatedSupervisionCohortListDto>>
{
    private readonly IAppLogger<GetAvailableSupervisorsQueryHandler> _logger;
    private readonly IUserApiService _userApiService;
    private readonly IUnitOfWork _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetAvailableSupervisorsQueryHandler(IAppLogger<GetAvailableSupervisorsQueryHandler> logger, IUserApiService userApiService, IUnitOfWork db, IHttpContextAccessor httpContextAccessor)
    {
        this._logger = logger;
        this._userApiService = userApiService;
        this._db = db;
        this._httpContextAccessor = httpContextAccessor;
    }

    public async Task<ResponseDto<PaginatedSupervisionCohortListDto>> Handle(GetAvailableSupervisorsQuery request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to retrieve list of Assigned Supervisors");
        var userId = this._httpContextAccessor.HttpContext?.Items["UserId"] as string;

        //get the student
        Domain.Entities.Student? student = await this._db.StudentRepository.GetFirstOrDefaultAsync(x => x.UserId == userId) ?? throw new NotFoundException(nameof(Student), userId ?? throw new InvalidOperationException());
        request.Parameters.DissertationCohortId = student.DissertationCohortId;

        return await this._userApiService.GetSupervisionCohorts(request.Parameters);
    }
}