using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using MediatR;
using Shared.DTO;
using Shared.Logging;
using UserManagement_API.Data.Models.Dto;

namespace Dissertation.Application.Supervisor.Commands.AssignAdminRole;

public class AssignAdminRoleCommandHandler : IRequestHandler<AssignAdminRoleCommand, ResponseDto<UserDto>>
{
    private readonly IAppLogger<AssignAdminRoleCommandHandler> _logger;
    private readonly IUserApiService _userApiService;

    public AssignAdminRoleCommandHandler(IAppLogger<AssignAdminRoleCommandHandler> logger, IUserApiService userApiService)
    {
        this._logger = logger;
        this._userApiService = userApiService;
    }

    public async Task<ResponseDto<UserDto>> Handle(AssignAdminRoleCommand request, CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to assign admin role to a supervisor");
        var userApiRequest = new AssignAdminRoleRequestDto()
        {
            Email = request.Email,
            Role = request.Role
        };
        return await this._userApiService.AssignAdminRoleToSupervisor(userApiRequest);
    }
}