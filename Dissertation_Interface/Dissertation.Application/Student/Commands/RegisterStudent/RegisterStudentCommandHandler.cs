using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Logging;

namespace Dissertation.Application.Student.Commands.RegisterStudent;

public class RegisterStudentCommandHandler : IRequestHandler<RegisterStudentCommand, ResponseDto<string>>
{
    private readonly IAppLogger<RegisterStudentCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IUserApiService _userApiService;

    public RegisterStudentCommandHandler(IAppLogger<RegisterStudentCommandHandler> logger, IUnitOfWork db, IUserApiService userApiService)
    {
        this._logger = logger;
        this._db = db;
        this._userApiService = userApiService;
    }

    public async Task<ResponseDto<string>> Handle(RegisterStudentCommand request, CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to register a student");

        //fetch the invitation with the invitation Code
        Domain.Entities.StudentInvite? studentInvite =
            await this._db.StudentInviteRepository.GetFirstOrDefaultAsync(x =>
                x.StudentId == request.StudentId && x.InvitationCode == request.InvitationCode);

        if (studentInvite != null && studentInvite.ExpiryDate.Date >= DateTime.UtcNow.Date)
        {
            //call the user api to attempt registration
            var registrationRequest = new StudentOrSupervisorRegistrationDto()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = studentInvite.Email,
                UserName = studentInvite.StudentId,
                Password = request.Password,
                CourseId = request.CourseId
            };

            ResponseDto<string> responseFromUserApi =
                await this._userApiService.RegisterStudent(registrationRequest);

            this._logger.LogInformation($"Registration Response Result from User API - {responseFromUserApi.Result}");
            this._logger.LogInformation($"Response - Is it Successful from User API - {responseFromUserApi.IsSuccess}");

            if (responseFromUserApi.IsSuccess && !string.IsNullOrEmpty(responseFromUserApi.Result))
            {

                //save the student details
                var student = Domain.Entities.Student.Create(
                    responseFromUserApi.Result,
                    request.CourseId,
                    studentInvite.DissertationCohortId
                );

                await this._db.StudentRepository.AddAsync(student);

                //delete the student invite and return a successful response
                this._db.StudentInviteRepository.Remove(studentInvite);
                await this._db.SaveAsync(cancellationToken);

                //return the response
                return responseFromUserApi;
            }

            //return the response
            return responseFromUserApi;
        }

        var response = new ResponseDto<string> { IsSuccess = false, Result = ErrorMessages.DefaultError, Message = "Invalid Invitation code" };
        return response;
    }
}