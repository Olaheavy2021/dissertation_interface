using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Enums;
using Shared.Helpers;
using Shared.Logging;

namespace Dissertation.Application.StudentInvite.Queries.GetListOfStudentInviteQueryHandler;

public class GetStudentInviteListQueryHandler : IRequestHandler<GetStudentInviteListQuery, ResponseDto<PaginatedStudentInvite>>
{
    private readonly IAppLogger<GetStudentInviteListQueryHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public GetStudentInviteListQueryHandler(IAppLogger<GetStudentInviteListQueryHandler> logger, IUnitOfWork db, IMapper mapper)
    {
        this._db = db;
        this._logger = logger;
        this._mapper = mapper;
    }

    public async Task<ResponseDto<PaginatedStudentInvite>> Handle(GetStudentInviteListQuery request,
        CancellationToken cancellationToken)
    {
        var response = new ResponseDto<PaginatedStudentInvite>();
        this._logger.LogInformation("Attempting to retrieve list of Student Invites");

        //get active cohort if filter is not passed
        if (request.Parameters.FilterByCohortId == 0)
        {
            Domain.Entities.DissertationCohort? cohort = await this._db.DissertationCohortRepository.GetActiveDissertationCohort();
            if (cohort == null)
            {
                response.IsSuccess = false;
                response.Message = "Kindly filter by the cohortId as there is no active cohort at the moment";

                return response;
            }

            request.Parameters.FilterByCohortId = cohort.Id;
        }

        PagedList<Domain.Entities.StudentInvite> studentInvites = this._db.StudentInviteRepository.GetListOfStudentInvites(request.Parameters);

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
        Domain.Entities.StudentInvite studentInvite)
    {
        GetDissertationCohort mappedCohort = this._mapper.Map<GetDissertationCohort>(studentInvite.DissertationCohort);
        var getStudentInvite = new GetStudentInvite
        {
            Id = studentInvite.Id,
            FirstName = studentInvite.FirstName,
            Status = DateTime.UtcNow.Date > studentInvite.ExpiryDate.Date ? DissertationConfigStatus.Expired : DissertationConfigStatus.Active,
            StudentId = studentInvite.StudentId,
            LastName = studentInvite.LastName,
            ExpiryDate = studentInvite.ExpiryDate,
            Email = studentInvite.Email,
            DissertationCohort = mappedCohort
        };
        return getStudentInvite;
    }

}