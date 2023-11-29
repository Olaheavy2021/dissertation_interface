using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Logging;

namespace Dissertation.Application.Supervisor.Commands.RegisterSupervisor;

public class RegisterSupervisorCommandHandler : IRequestHandler<RegisterSupervisorCommand, ResponseDto<string>>
{
    private readonly IAppLogger<RegisterSupervisorCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IUserApiService _userApiService;

    public RegisterSupervisorCommandHandler(IAppLogger<RegisterSupervisorCommandHandler> logger, IUnitOfWork db, IUserApiService userApiService)
    {
        this._logger = logger;
        this._db = db;
        this._userApiService = userApiService;
    }

    public async Task<ResponseDto<string>> Handle(RegisterSupervisorCommand request, CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to register a supervisor");
        //fetch the invitation with the invitation Code
        Domain.Entities.SupervisorInvite? supervisorInvite =
            await this._db.SupervisorInviteRepository.GetFirstOrDefaultAsync(x =>
                x.StaffId == request.StaffId && x.InvitationCode == request.InvitationCode);

        if (supervisorInvite != null && supervisorInvite.ExpiryDate.Date >= DateTime.UtcNow.Date)
        {
            //call the user api to attempt registration
            var registrationRequest = new StudentOrSupervisorRegistrationDto()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = supervisorInvite.Email,
                UserName = supervisorInvite.StaffId,
                Password = request.Password,
            };

            ResponseDto<string> responseFromUserApi =
                await this._userApiService.RegisterSupervisor(registrationRequest);

            this._logger.LogInformation($"Registration Response Result from User API - {responseFromUserApi.Result}");
            this._logger.LogInformation($"Response - Is it Successful from User API - {responseFromUserApi.IsSuccess}");

            if (responseFromUserApi.IsSuccess && !string.IsNullOrEmpty(responseFromUserApi.Result))
            {

                //save the supervisor details
                var supervisor = Domain.Entities.Supervisor.Create(
                    responseFromUserApi.Result,
                    request.DepartmentId
                );

                await this._db.SupervisorRepository.AddAsync(supervisor);

                //delete the supervisor invite and return a successful response
                this._db.SupervisorInviteRepository.Remove(supervisorInvite);
                await this._db.SaveAsync(cancellationToken);

                //return the response
                return responseFromUserApi;
            }

            //return the response
            return responseFromUserApi;
        }

        var response = new ResponseDto<string> { IsSuccess = false, Result = ErrorMessages.DefaultError, Message = "Invalid Invitation code"};
        return response;
    }
}