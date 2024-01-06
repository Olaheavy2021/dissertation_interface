using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.Supervisor.Commands.UpdateSupervisor;

public class UpdateSupervisorCommandHandler : IRequestHandler<UpdateSupervisorCommand, ResponseDto<UserDto>>
{
    private readonly IAppLogger<UpdateSupervisorCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IUserApiService _userApiService;

    public UpdateSupervisorCommandHandler(IAppLogger<UpdateSupervisorCommandHandler> logger, IUnitOfWork db, IUserApiService userApiService)
    {
        this._logger = logger;
        this._db = db;
        this._userApiService = userApiService;
    }
    public async Task<ResponseDto<UserDto>> Handle(UpdateSupervisorCommand request, CancellationToken cancellationToken)
    {
        //fetch the supervision invite from the database
        Domain.Entities.Supervisor? supervisor = await this._db.SupervisorRepository.GetFirstOrDefaultAsync(a => a.Id == request.Id);
        if (supervisor == null)
        {
            this._logger.LogError("No Supervisor found with {ID}", request.Id);
            throw new NotFoundException(nameof(Domain.Entities.Supervisor), request.Id);
        }

        //update on the user api.
        var userApiRequest = new EditSupervisorRequestDto()
        {
            FirstName = request.FirstName,
            StaffId = request.StaffId,
            LastName = request.LastName,
            UserId = supervisor.UserId,
            DepartmentId = request.DepartmentId
        };
        ResponseDto<UserDto> userResponse = await this._userApiService.EditSupervisor(userApiRequest);

        if (!userResponse.IsSuccess)
        {
            return new ResponseDto<UserDto> { IsSuccess = false, Message = userResponse.Message };
        }

        //update the department if necessary
        if (supervisor.DepartmentId == request.DepartmentId)
            return new ResponseDto<UserDto>()
            {
                IsSuccess = true,
                Message = SuccessMessages.DefaultSuccess,
                Result = userResponse.Result
            };

        supervisor.DepartmentId = request.DepartmentId;
        this._db.SupervisorRepository.Update(supervisor);
        await this._db.SaveAsync(cancellationToken);

        return new ResponseDto<UserDto>
        {
            IsSuccess = true,
            Message = SuccessMessages.DefaultSuccess,
            Result = userResponse.Result
        };
    }
}