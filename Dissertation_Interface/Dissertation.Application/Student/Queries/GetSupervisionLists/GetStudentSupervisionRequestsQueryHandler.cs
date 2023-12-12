using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.DTO;
using Shared.Exceptions;

namespace Dissertation.Application.Student.Queries.GetSupervisionLists;

public class GetSupervisionListQueryHandler : IRequestHandler<GetStudentSupervisionListsQuery, ResponseDto<PaginatedSupervisionListDto>>
{
    private readonly ILogger<GetSupervisionListQueryHandler> _logger;
    private readonly IUserApiService _userApiService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetSupervisionListQueryHandler(ILogger<GetSupervisionListQueryHandler> logger, IUserApiService userApiService, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        this._logger = logger;
        this._userApiService = userApiService;
        this._unitOfWork = unitOfWork;
        this._httpContextAccessor = httpContextAccessor;
    }

    public async Task<ResponseDto<PaginatedSupervisionListDto>> Handle(GetStudentSupervisionListsQuery request,
        CancellationToken cancellationToken)
    {
        //fetch the student from the database
        var userId = this._httpContextAccessor.HttpContext?.Items["UserId"] as string;
        if (userId == null)
        {
            this._logger.LogError("Invalid token passed to fetch list of supervision request");
            throw new NotFoundException("HttpContext", "UserId");
        }
        Domain.Entities.Student? student = await this._unitOfWork.StudentRepository.GetFirstOrDefaultAsync(a => a.UserId == userId, includes: x => x.Course);
        if (student == null)
        {
            this._logger.LogError($"No Student found with {userId}", userId);
            throw new NotFoundException(nameof(Domain.Entities.Student), userId);
        }

        var apiRequest = new SupervisionListPaginationParameters()
        {
            SearchBySupervisor = request.Parameters.SearchBySupervisor,
            DissertationCohortId = student.DissertationCohortId,
            PageNumber = request.Parameters.PageNumber,
            PageSize = request.Parameters.PageSize,
            StudentId = student.UserId
        };

        ResponseDto<PaginatedSupervisionListDto> response = await this._userApiService.GetSupervisionListsForStudents(apiRequest);
        return response;
    }
}