using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.StudentInvite.Queries.GetById;

public class GetStudentInviteByIdQueryHandler : IRequestHandler<GetStudentInviteByIdQuery, ResponseDto<GetStudentInvite>>
{
    private readonly IAppLogger<GetStudentInviteByIdQueryHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public GetStudentInviteByIdQueryHandler(IAppLogger<GetStudentInviteByIdQueryHandler> logger, IUnitOfWork db, IMapper mapper)
    {
        this._logger = logger;
        this._db = db;
        this._mapper = mapper;
    }

    public async Task<ResponseDto<GetStudentInvite>> Handle(GetStudentInviteByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new ResponseDto<GetStudentInvite>();
        this._logger.LogInformation("Attempting to retrieve a Student Invite by ID {StudentInviteID}", request.Id);
        Domain.Entities.StudentInvite? studentInvite = await this._db.StudentInviteRepository.GetAsync(a => a.Id == request.Id, includes: x => x.DissertationCohort);
        if (studentInvite is null)
        {
            this._logger.LogError("No Student Invite with ID");
            throw new NotFoundException(nameof(Domain.Entities.StudentInvite), request.Id);
        }

        GetStudentInvite mappedStudentInvite = this._mapper.Map<GetStudentInvite>(studentInvite);
        mappedStudentInvite.UpdateStatus();

        this._logger.LogInformation("Successfully retrieved a Student Invite by ID {StudentInviteID}", request.Id);
        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedStudentInvite;
        return response;
    }
}