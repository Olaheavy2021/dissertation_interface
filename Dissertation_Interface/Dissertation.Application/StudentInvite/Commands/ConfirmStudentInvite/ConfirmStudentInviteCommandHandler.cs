using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Logging;

namespace Dissertation.Application.StudentInvite.Commands.ConfirmStudentInvite;

public class ConfirmStudentInviteCommandHandler : IRequestHandler<ConfirmStudentInviteCommand, ResponseDto<GetStudentInvite>>
{
    private readonly IAppLogger<ConfirmStudentInviteCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public ConfirmStudentInviteCommandHandler(IAppLogger<ConfirmStudentInviteCommandHandler> logger, IUnitOfWork db, IMapper mapper)
    {
        this._logger = logger;
        this._db = db;
        this._mapper = mapper;
    }
    public async Task<ResponseDto<GetStudentInvite>> Handle(ConfirmStudentInviteCommand request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to Confirm Student Invite for {username}", request.StudentId);
        var response = new ResponseDto<GetStudentInvite>();
        Domain.Entities.StudentInvite? studentInvite =
            await this._db.StudentInviteRepository.GetFirstOrDefaultAsync(x =>
                x.StudentId == request.StudentId && x.InvitationCode == request.InvitationCode, includes: x=> x.DissertationCohort);

        if (studentInvite == null)
        {
            response.IsSuccess = false;
            response.Message = "Invalid Invitation Code. Please contact admin";
            return response;
        }

        if (DateTime.UtcNow.Date > studentInvite.ExpiryDate.Date)
        {
            response.IsSuccess = false;
            response.Message = "Invitation Code has Expired. Please contact admin";
            return response;
        }

        GetStudentInvite mappedStudentInvite = this._mapper.Map<GetStudentInvite>(studentInvite);
        mappedStudentInvite.UpdateStatus();

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedStudentInvite;

        return response;
    }
}