using Dissertation.Application.Supervisor.Commands.AssignAdminRole;
using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using MediatR;
using Shared.DTO;
using Shared.Logging;

namespace Dissertation.Application.Supervisor.Commands.AssignSupervisorRole;

public class AssignSupervisorRoleCommandHandler : IRequestHandler<AssignSupervisorRoleCommand, ResponseDto<UserDto>>
{
    private readonly IAppLogger<AssignAdminRoleCommandHandler> _logger;
    private readonly IUserApiService _userApiService;
    private readonly IUnitOfWork _db;

    public AssignSupervisorRoleCommandHandler(IAppLogger<AssignAdminRoleCommandHandler> logger,
        IUserApiService userApiService, IUnitOfWork db)
    {
        this._logger = logger;
        this._userApiService = userApiService;
        this._db = db;
    }

    public async Task<ResponseDto<UserDto>> Handle(AssignSupervisorRoleCommand request,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to assign admin role to a supervisor");
        var userApiRequest = new AssignSupervisorRoleRequestDto()
        {
            Email = request.Email,
            DepartmentId = request.DepartmentId
        };
        ResponseDto<UserDto> userApiResponse = await this._userApiService.AssignSupervisorRoleToAdmin(userApiRequest);

        if (userApiResponse.IsSuccess && userApiResponse.Result != null)
        {
            //save details into the database
            var supervisor = Domain.Entities.Supervisor.Create(
                userApiResponse.Result.Id,
                request.DepartmentId
            );

            await this._db.SupervisorRepository.AddAsync(supervisor);
            await this._db.SaveAsync(cancellationToken);

            return userApiResponse;
        }

        return userApiResponse;
    }
}