using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.StudentInvite.Commands.DeleteStudentInvite;

public class DeleteStudentInviteCommandHandler : IRequestHandler<DeleteStudentInviteCommand, ResponseDto<GetStudentInvite>>
{
    private readonly IAppLogger<DeleteStudentInviteCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public DeleteStudentInviteCommandHandler(IAppLogger<DeleteStudentInviteCommandHandler> logger, IUnitOfWork db,
        IMapper mapper)
    {
        this._logger = logger;
        this._mapper = mapper;
        this._db = db;
    }

    public async Task<ResponseDto<GetStudentInvite>> Handle(DeleteStudentInviteCommand request,
        CancellationToken cancellationToken)
    {
        var response = new ResponseDto<GetStudentInvite>();

        //fetch the student invite from the database
        Domain.Entities.StudentInvite? studentInvite =
            await this._db.StudentInviteRepository.GetFirstOrDefaultAsync(a => a.Id == request.Id, includes: x => x.DissertationCohort);

        if (studentInvite == null)
        {
            this._logger.LogError("No Student Invite found with {ID}", request.Id);
            throw new NotFoundException(nameof(Domain.Entities.StudentInvite), request.Id);
        }

        this._db.StudentInviteRepository.Remove(studentInvite);
        await this._db.SaveAsync(cancellationToken);

        GetStudentInvite mappedStudentInvite = this._mapper.Map<GetStudentInvite>(studentInvite);
        mappedStudentInvite.UpdateStatus();

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedStudentInvite;

        return response;
    }
}