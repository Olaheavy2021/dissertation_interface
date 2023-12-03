using AutoMapper;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Shared.Constants;
using Shared.DTO;
using Shared.Enums;
using Shared.Exceptions;
using Shared.Helpers;
using Shared.Logging;
using Shared.MessageBus;
using Shared.Settings;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;
using UserManagement_API.Data.Models.Dto;
using UserManagement_API.Data.Models.Validators;
using UserManagement_API.Service.IService;

namespace UserManagement_API.Service;

public class UserService : IUserService
{
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;
    private readonly IAppLogger<UserService> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMessageBus _messageBus;
    private readonly ServiceBusSettings _serviceBusSettings;

    public UserService(IUnitOfWork db, IAppLogger<UserService> logger, IMapper mapper, UserManager<ApplicationUser> userManager, IMessageBus messageBus, IOptions<ServiceBusSettings> serviceBusSettings)
    {
        this._db = db;
        this._mapper = mapper;
        this._logger = logger;
        this._userManager = userManager;
        this._messageBus = messageBus;
        this._serviceBusSettings = serviceBusSettings.Value;
    }

    public async Task<ResponseDto<GetUserDto>> GetUser(string userId)
    {
        var response = new ResponseDto<GetUserDto>();

        ApplicationUser? user = await this._db.ApplicationUserRepository.GetFirstOrDefaultAsync(a => a.Id == userId);
        this._logger.LogInformation("Fetching details of this user with userId from the database - {0}", userId);
        if (user != null)
        {
            IList<string> roles = await this._userManager.GetRolesAsync(user);
            UserDto mappedUser = this._mapper.Map<ApplicationUser, UserDto>(user);
            mappedUser.Status = user.LockoutEnd >= DateTimeOffset.UtcNow
                ? UserStatus.Deactivated
                : user.EmailConfirmed ? UserStatus.Active : UserStatus.Inactive;

            var getUserDto = new GetUserDto() { User = mappedUser, Role = roles, IsLockedOut = user.LockoutEnd >= DateTimeOffset.UtcNow };

            response.IsSuccess = true;
            response.Message = SuccessMessages.DefaultSuccess;
            response.Result = getUserDto;
            this._logger.LogInformation("User Details returned successfully for userId - {0}", userId);
            return response;
        }

        response.IsSuccess = false;
        response.Message = "No user found";
        this._logger.LogInformation("User Details not found for userId - {0}", userId);
        return response;
    }

    public async Task<ResponseDto<GetUserDto>> GetUserByEmail(string email)
    {
        var response = new ResponseDto<GetUserDto>();

        ApplicationUser? user = await this._userManager.FindByEmailAsync(email);
        this._logger.LogInformation("Fetching details of this user with email from the database - {0}", email);
        if (user != null)
        {
            IList<string> roles = await this._userManager.GetRolesAsync(user);
            UserDto mappedUser = this._mapper.Map<ApplicationUser, UserDto>(user);
            var getUserDto = new GetUserDto() { User = mappedUser, Role = roles, IsLockedOut = user.LockoutEnd >= DateTimeOffset.UtcNow };

            response.IsSuccess = true;
            response.Message = SuccessMessages.DefaultSuccess;
            response.Result = getUserDto;
            this._logger.LogInformation("User Details returned successfully for email - {0}", email);
            return response;
        }

        response.IsSuccess = false;
        response.Message = "No user found";
        this._logger.LogInformation("User Details not found for email - {0}", email);
        return response;
    }

    public async Task<ResponseDto<GetUserDto>> GetUserByUserName(string userName)
    {
        var response = new ResponseDto<GetUserDto>();

        ApplicationUser? user = await this._userManager.FindByNameAsync(userName);
        this._logger.LogInformation("Fetching details of this user with username from the database - {0}", userName);
        if (user != null)
        {
            IList<string> roles = await this._userManager.GetRolesAsync(user);
            UserDto mappedUser = this._mapper.Map<ApplicationUser, UserDto>(user);
            var getUserDto = new GetUserDto() { User = mappedUser, Role = roles, IsLockedOut = user.LockoutEnd >= DateTimeOffset.UtcNow };

            response.IsSuccess = true;
            response.Message = SuccessMessages.DefaultSuccess;
            response.Result = getUserDto;
            this._logger.LogInformation("User Details returned successfully for username - {0}", userName);
            return response;
        }

        response.IsSuccess = false;
        response.Message = "No user found";
        this._logger.LogInformation("User Details not found for username - {0}", userName);
        return response;
    }

    public async Task<ResponseDto<bool>> LockOutUser(string email, string? loggedInAdminEmail)
    {
        var response = new ResponseDto<bool> { IsSuccess = false, Result = false, Message = "An error occurred whilst locking out the user" };
        this._logger.LogInformation("Request to lock out this user with this email -  {0}", email);
        ApplicationUser? user = await this._userManager.FindByEmailAsync(email);
        if (user == null) return response;

        if (user.UserName != null && (user.UserName.Equals(SystemDefault.DefaultSuperAdmin1) ||
                                      user.UserName.Equals(SystemDefault.DefaultSuperAdmin2)))
        {
            response.IsSuccess = false;
            response.Message = "Lock out failed. This user is a system default user";
            await this._messageBus.PublishAuditLog(EventType.LockOutUser, this._serviceBusSettings.ServiceBusConnectionString,
                loggedInAdminEmail, ErrorMessages.DefaultError, email);
            return response;
        }


        IdentityResult lockDate = await this._userManager.SetLockoutEndDateAsync(user, SystemDefault.LockOutEndDate);
        user.IsLockedOutByAdmin = true;
        await this._userManager.UpdateAsync(user);

        response.IsSuccess = lockDate.Succeeded;
        response.Result = lockDate.Succeeded;
        response.Message = "User locked out successfully";
        await this._messageBus.PublishAuditLog(EventType.LockOutUser, this._serviceBusSettings.ServiceBusConnectionString,
            loggedInAdminEmail, SuccessMessages.DefaultSuccess, email);
        await PublishAccountDeactivationOrActivationEmail(user, EmailType.EmailTypeAccountDeactivationEmail);
        this._logger.LogInformation("User lock out processed with this response - {0}", lockDate.Succeeded);
        return response;
    }

    public async Task<ResponseDto<bool>> UnlockUser(string email, string? loggedInAdminEmail)
    {
        var response = new ResponseDto<bool> { IsSuccess = false, Result = false, Message = "An error occurred whilst unlocking the user" };
        this._logger.LogInformation("Request to unlock user with this email -  {0}", email);
        ApplicationUser? user = await this._userManager.FindByEmailAsync(email);
        if (user == null) return response;

        IdentityResult setLockoutEndDate = await this._userManager.SetLockoutEndDateAsync(user, DateTime.Now - TimeSpan.FromMinutes(1));

        if (user.IsLockedOutByAdmin)
        {
            user.IsLockedOutByAdmin = false;
            await this._userManager.UpdateAsync(user);
        }

        response.IsSuccess = setLockoutEndDate.Succeeded;
        response.Result = setLockoutEndDate.Succeeded;
        response.Message = "User unlocked successfully";
        await this._messageBus.PublishAuditLog(EventType.UnlockUser, this._serviceBusSettings.ServiceBusConnectionString,
            loggedInAdminEmail, SuccessMessages.DefaultSuccess, email);
        await PublishAccountDeactivationOrActivationEmail(user, EmailType.EmailTypeAccountActivationEmail);
        this._logger.LogInformation("User unlock processed with this response - {0}", setLockoutEndDate.Succeeded);

        return response;

    }

    public ResponseDto<PaginatedUserListDto> GetPaginatedAdminUsers(UserPaginationParameters paginationParameters)
    {
        var response = new ResponseDto<PaginatedUserListDto>();
        PagedList<ApplicationUser> users = this._db.ApplicationUserRepository.GetPaginatedAdminUsers(paginationParameters);

        var userDtos = new PagedList<UserListDto>(
            users.Select(MapToUserDto).ToList(),
            users.TotalCount,
            users.CurrentPage,
            users.PageSize
        );

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = new PaginatedUserListDto
        {
            Data = userDtos,
            TotalCount = userDtos.TotalCount,
            PageSize = userDtos.PageSize,
            CurrentPage = userDtos.CurrentPage,
            TotalPages = userDtos.TotalPages,
            HasNext = userDtos.HasNext,
            HasPrevious = userDtos.HasPrevious
        };

        return response;
    }

    public ResponseDto<PaginatedUserListDto> GetPaginatedStudents(DissertationStudentPaginationParameters paginationParameters)
    {
        var response = new ResponseDto<PaginatedUserListDto>();
        PagedList<ApplicationUser> users = this._db.ApplicationUserRepository.GetPaginatedStudents(paginationParameters);

        var userDtos = new PagedList<UserListDto>(
            users.Select(MapToUserDto).ToList(),
            users.TotalCount,
            users.CurrentPage,
            users.PageSize
        );

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = new PaginatedUserListDto
        {
            Data = userDtos,
            TotalCount = userDtos.TotalCount,
            PageSize = userDtos.PageSize,
            CurrentPage = userDtos.CurrentPage,
            TotalPages = userDtos.TotalPages,
            HasNext = userDtos.HasNext,
            HasPrevious = userDtos.HasPrevious
        };

        return response;
    }

    public ResponseDto<PaginatedUserListDto> GetPaginatedSupervisors(
        SupervisorPaginationParameters paginationParameters)
    {
        var response = new ResponseDto<PaginatedUserListDto>();
        PagedList<ApplicationUser> users = this._db.ApplicationUserRepository.GetPaginatedSupervisors(paginationParameters);

        var userDtos = new PagedList<UserListDto>(
            users.Select(MapToUserDto).ToList(),
            users.TotalCount,
            users.CurrentPage,
            users.PageSize
        );

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = new PaginatedUserListDto
        {
            Data = userDtos,
            TotalCount = userDtos.TotalCount,
            PageSize = userDtos.PageSize,
            CurrentPage = userDtos.CurrentPage,
            TotalPages = userDtos.TotalPages,
            HasNext = userDtos.HasNext,
            HasPrevious = userDtos.HasPrevious
        };

        return response;
    }

    public async Task<ResponseDto<EditUserRequestDto>> EditUser(EditUserRequestDto request, string? loggedInAdminEmail)
    {
        if (string.IsNullOrEmpty(loggedInAdminEmail))
            throw new UnauthorizedException();

        var response = new ResponseDto<EditUserRequestDto> { IsSuccess = false, Message = ErrorMessages.DefaultError };
        this._logger.LogInformation("Request to update user with this email -  {0}", request.Email);

        //validate the request
        var validator = new EditUserRequestDtoValidator();
        ValidationResult? validationResult = await validator.ValidateAsync(request);
        if (validationResult.Errors.Any())
        {
            this._logger.LogWarning("Edit User - Validation errors in whilst editing user for {0} - {1}", nameof(ApplicationUser), request.UserName);
            await this._messageBus.PublishAuditLog(EventType.EditUser,
                this._serviceBusSettings.ServiceBusConnectionString, loggedInAdminEmail, ErrorMessages.DefaultError, request.Email);
            throw new BadRequestException("Invalid Login Request", validationResult);
        }

        //check if the user exists
        ApplicationUser? user = await this._userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            response.Message = $"User with this userid - {request.UserId} does not exist";
            await this._messageBus.PublishAuditLog(EventType.EditUser,
                this._serviceBusSettings.ServiceBusConnectionString, loggedInAdminEmail, ErrorMessages.DefaultError, request.Email);
            return response;
        }

        //check if the user's email has been confirmed
        var isEmailConfirmed = await this._userManager.IsEmailConfirmedAsync(user);

        //check if the email is unique
        if (user is { Email: not null } && !user.Email.Equals(request.Email))
        {
            var doesUserDetailsExist = await this._db.ApplicationUserRepository.DoesEmailExist(request.Email, new CancellationToken());
            if (doesUserDetailsExist)
            {
                response.IsSuccess = false;
                response.Message = "Email  already exists for another user";
                await this._messageBus.PublishAuditLog(EventType.EditUser,
                    this._serviceBusSettings.ServiceBusConnectionString, loggedInAdminEmail, ErrorMessages.DefaultError, request.Email);
                return response;
            }
        }

        //check if the username is unique
        if (user is { UserName: not null } && !user.UserName.Equals(request.UserName))
        {
            var doesUserDetailsExist = await this._db.ApplicationUserRepository.DoesUserNameExist(request.UserName, new CancellationToken());
            if (doesUserDetailsExist)
            {
                response.IsSuccess = false;
                response.Message = "Username already exists for another user";
                await this._messageBus.PublishAuditLog(EventType.EditUser,
                    this._serviceBusSettings.ServiceBusConnectionString, loggedInAdminEmail, ErrorMessages.DefaultError, request.Email);
                return response;
            }
        }
        if (isEmailConfirmed)
        {
            user.UserName = request.UserName;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
        }
        else
        {
            user.UserName = request.UserName;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
        }

        await this._userManager.UpdateAsync(user);
        this._logger.LogInformation("User updated successfully -  {0}", request.Email);
        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = request;
        await this._messageBus.PublishAuditLog(EventType.EditUser,
            this._serviceBusSettings.ServiceBusConnectionString, loggedInAdminEmail, SuccessMessages.DefaultSuccess, request.Email);
        return response;
    }

    public async Task<ResponseDto<UserDto>> EditSupervisor(EditSupervisorRequestDto request, string? loggedInUser)
    {
        if (string.IsNullOrEmpty(loggedInUser))
            throw new UnauthorizedException();

        var response = new ResponseDto<UserDto> { IsSuccess = false, Message = ErrorMessages.DefaultError };
        this._logger.LogInformation("Request to update supervisor with this userId -  {0}", request.UserId);

        //validate the request
        var validator = new EditSupervisorRequestDtoValidator();
        ValidationResult? validationResult = await validator.ValidateAsync(request);
        if (validationResult.Errors.Any())
        {
            this._logger.LogWarning("Edit Supervisor - Validation errors in whilst editing user for {0} - {1}", nameof(ApplicationUser), request.StaffId);
            await this._messageBus.PublishAuditLog(EventType.EditUser,
                this._serviceBusSettings.ServiceBusConnectionString, loggedInUser, ErrorMessages.DefaultError, request.StaffId);
            throw new BadRequestException("Invalid Login Request", validationResult);
        }

        //check if the user exists
        ApplicationUser? user = await this._userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            response.Message = $"Supervisor with this userid - {request.UserId} does not exist";
            await this._messageBus.PublishAuditLog(EventType.EditUser,
                this._serviceBusSettings.ServiceBusConnectionString, loggedInUser, ErrorMessages.DefaultError, request.StaffId);
            return response;
        }

        //check if the username is unique
        if (user is { UserName: not null } && !user.UserName.Equals(request.StaffId))
        {
            var doesUserDetailsExist = await this._db.ApplicationUserRepository.DoesUserNameExist(request.StaffId, new CancellationToken());
            if (doesUserDetailsExist)
            {
                response.IsSuccess = false;
                response.Message = "Username already exists for another user";
                await this._messageBus.PublishAuditLog(EventType.EditUser,
                    this._serviceBusSettings.ServiceBusConnectionString, loggedInUser, ErrorMessages.DefaultError, request.StaffId);
                return response;
            }
        }

        user.UserName = request.StaffId;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.DepartmentId = request.DepartmentId;

        await this._userManager.UpdateAsync(user);

        UserDto? mappedUser = this._mapper.Map<UserDto>(user);
        this._logger.LogInformation("Supervisor updated successfully -  {0}", request.UserId);
        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedUser;
        await this._messageBus.PublishAuditLog(EventType.EditUser,
            this._serviceBusSettings.ServiceBusConnectionString, loggedInUser, SuccessMessages.DefaultSuccess, request.StaffId);
        return response;
    }

    public async Task<ResponseDto<UserDto>> EditStudent(EditStudentRequestDto request, string? loggedInUser)
    {
        if (string.IsNullOrEmpty(loggedInUser))
            throw new UnauthorizedException();

        var response = new ResponseDto<UserDto> { IsSuccess = false, Message = ErrorMessages.DefaultError };
        this._logger.LogInformation("Request to update student with this userId -  {0}", request.UserId);

        //validate the request
        var validator = new EditStudentRequestDtoValidator();
        ValidationResult? validationResult = await validator.ValidateAsync(request);
        if (validationResult.Errors.Any())
        {
            this._logger.LogWarning("Edit Student - Validation errors in whilst editing user for {0} - {1}", nameof(ApplicationUser), request.StudentId);
            await this._messageBus.PublishAuditLog(EventType.EditUser,
                this._serviceBusSettings.ServiceBusConnectionString, loggedInUser, ErrorMessages.DefaultError, request.StudentId);
            throw new BadRequestException("Invalid Login Request", validationResult);
        }

        //check if the user exists
        ApplicationUser? user = await this._userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            response.Message = $"Student with this userid - {request.UserId} does not exist";
            await this._messageBus.PublishAuditLog(EventType.EditUser,
                this._serviceBusSettings.ServiceBusConnectionString, loggedInUser, ErrorMessages.DefaultError, request.StudentId);
            return response;
        }

        //check if the username is unique
        if (user is { UserName: not null } && !user.UserName.Equals(request.StudentId))
        {
            var doesUserDetailsExist = await this._db.ApplicationUserRepository.DoesUserNameExist(request.StudentId, new CancellationToken());
            if (doesUserDetailsExist)
            {
                response.IsSuccess = false;
                response.Message = "Username already exists for another user";
                await this._messageBus.PublishAuditLog(EventType.EditUser,
                    this._serviceBusSettings.ServiceBusConnectionString, loggedInUser, ErrorMessages.DefaultError, request.StudentId);
                return response;
            }
        }

        user.UserName = request.StudentId;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.CourseId = request.CourseId;

        await this._userManager.UpdateAsync(user);

        UserDto? mappedUser = this._mapper.Map<UserDto>(user);
        this._logger.LogInformation("Student updated successfully -  {0}", request.UserId);
        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedUser;
        await this._messageBus.PublishAuditLog(EventType.EditUser,
            this._serviceBusSettings.ServiceBusConnectionString, loggedInUser, SuccessMessages.DefaultSuccess, request.StudentId);
        return response;
    }


    private static UserListDto MapToUserDto(ApplicationUser applicationUser) =>
        new()
        {
            Id = applicationUser.Id,
            Email = applicationUser.Email,
            UserName = applicationUser.UserName,
            FirstName = applicationUser.FirstName,
            LastName = applicationUser.LastName,
            IsLockedOut = applicationUser.LockoutEnd >= DateTimeOffset.UtcNow,
            EmailConfirmed = applicationUser.EmailConfirmed,
            Status = applicationUser.LockoutEnd >= DateTimeOffset.UtcNow
                ? UserStatus.Deactivated
                : applicationUser.EmailConfirmed ? UserStatus.Active : UserStatus.Inactive
        };

    private async Task PublishAccountDeactivationOrActivationEmail(ApplicationUser user, string emailType)
    {
        UserDto? userToReturn = this._mapper.Map<UserDto>(user);
        this._logger.LogInformation("Account activation or deactivation Email Published for this user {0}", user.UserName ?? string.Empty);

        var emailDto = new PublishEmailDto { User = userToReturn, EmailType = emailType };
        await this._messageBus.PublishMessage(emailDto, this._serviceBusSettings.EmailLoggerQueue,
            this._serviceBusSettings.ServiceBusConnectionString);
    }
}