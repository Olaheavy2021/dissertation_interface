using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Enums;
using Dissertation.Infrastructure.Persistence.IRepository;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Helpers;
using Shared.Logging;

namespace Dissertation.Application.StudentInvite.Queries.GetListOfStudentInviteQueryHandler;

public class GetStudentInviteListQueryHandler : IRequestHandler<GetStudentInviteListQuery, ResponseDto<PaginatedStudentInvite>>
{
    private readonly IAppLogger<GetStudentInviteListQueryHandler> _logger;
    private readonly IUnitOfWork _db;

    public GetStudentInviteListQueryHandler(IAppLogger<GetStudentInviteListQueryHandler> logger, IUnitOfWork db)
    {
        this._db = db;
        this._logger = logger;
    }

    public async Task<ResponseDto<PaginatedStudentInvite>> Handle(GetStudentInviteListQuery request,
        CancellationToken cancellationToken)
    {
        var response = new ResponseDto<PaginatedStudentInvite>();
        this._logger.LogInformation("Attempting to retrieve list of Student Invites");

        //get active cohort
        Domain.Entities.DissertationCohort? cohort = await this._db.DissertationCohortRepository.GetActiveDissertationCohort();
        if (cohort == null)
        {
            response.IsSuccess = false;
            response.Message = "Kindly initiate a new and active dissertation cohort before inviting Students";

            return response;
        }
        PagedList<Domain.Entities.StudentInvite> studentInvites = this._db.StudentInviteRepository.GetListOfStudentInvites(request.Parameters, cohort.Id);

        var mappedStudentInvite = new PagedList<GetStudentInvite>(
            studentInvites.Select(MapToStudentInviteDto).ToList(),
            studentInvites.TotalCount,
            studentInvites.CurrentPage,
            studentInvites.PageSize
        );

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = new PaginatedStudentInvite()
        {
            Data = mappedStudentInvite,
            TotalCount = mappedStudentInvite.TotalCount,
            PageSize = mappedStudentInvite.PageSize,
            CurrentPage = mappedStudentInvite.CurrentPage,
            TotalPages = mappedStudentInvite.TotalPages,
            HasNext = mappedStudentInvite.HasNext,
            HasPrevious = mappedStudentInvite.HasPrevious
        };

        return response;
    }

    private GetStudentInvite MapToStudentInviteDto(
        Domain.Entities.StudentInvite studentInvite) =>
        new()
        {
            Id = studentInvite.Id,
            FirstName = studentInvite.FirstName,
            Status = DateTime.UtcNow.Date > studentInvite.ExpiryDate.Date ? DissertationConfigStatus.Expired : DissertationConfigStatus.Active,
            StudentId = studentInvite.StudentId,
            LastName = studentInvite.LastName,
            ExpiryDate = studentInvite.ExpiryDate,
        };
}