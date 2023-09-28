using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Shared.Constants;
using Shared.Enums;
using Shared.Logging;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;
using UserManagement_API.Data.Models.Dto;
using UserManagement_API.Service.IService;

namespace UserManagement_API.Service;

public class UserService : IUserService
{
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;
    private readonly IAppLogger<UserService> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(IUnitOfWork db, IAppLogger<UserService> logger, IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        this._db = db;
        this._mapper = mapper;
        this._logger = logger;
        this._userManager = userManager;
    }

    public async Task<ResponseDto<List<UserDto>>> GetAdminUsers()
    {
        var response = new ResponseDto<List<UserDto>>();
        var users = new List<ApplicationUser>();
        this._logger.LogInformation("Fetching all the admin users on the system");

        IList<ApplicationUser> adminUsers = await this._userManager.GetUsersInRoleAsync(RolesEnum.AdminRoles.Admin.ToString());
        if (adminUsers.Any())
            users.AddRange(adminUsers);
        IList<ApplicationUser> superAdminUsers = await this._userManager.GetUsersInRoleAsync(RolesEnum.AdminRoles.Superadmin.ToString());
        if (superAdminUsers.Any())
        {
            //filter out system default users
            IEnumerable<ApplicationUser> filteredSuperAdminUsers = superAdminUsers.Where(s =>
                 s.UserName != SystemDefault.DefaultSuperAdmin1 || s.UserName != SystemDefault.DefaultSuperAdmin2);
            users.AddRange(filteredSuperAdminUsers);
        }

        if (users.Any())
        {
            List<UserDto> mappedUsers = this._mapper.Map<List<ApplicationUser>, List<UserDto>>(users);
            response.IsSuccess = true;
            response.Message = SuccessMessages.DefaultSuccess;
            response.Result = mappedUsers;
            return response;
        }

        response.IsSuccess = false;
        response.Message = "No admin users found.";
        response.Result = new List<UserDto>();
        this._logger.LogInformation("Admin users fetched successfully, {0} records returned", users.Count);
        return response;
    }

    public async Task<ResponseDto<GetUserDto>> GetUser(string userId)
    {
        var response = new ResponseDto<GetUserDto>();
        ApplicationUser? user = await this._db.ApplicationUserRepository.GetFirstOrDefaultAsync(a => a.Id == userId);
        this._logger.LogInformation("Fetching details of this user with userId - {0}", userId);
        if (user != null)
        {
            IList<string> roles = await this._userManager.GetRolesAsync(user);
            var isLockedOut = await this._userManager.IsLockedOutAsync(user);
            UserDto mappedUser = this._mapper.Map<ApplicationUser, UserDto>(user);
            var getUserDto = new GetUserDto() { User = mappedUser, Role = roles.FirstOrDefault() ?? string.Empty, IsLockedOut = isLockedOut };

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

    public async Task<ResponseDto<bool>> LockOutUser(string email)
    {
        var response = new ResponseDto<bool> { IsSuccess = false, Result = false, Message = "An error occurred whilst locking out the user" };
        this._logger.LogInformation("Request to lock out this user with this email -  {0}", email);
        ApplicationUser? user = await this._userManager.FindByEmailAsync(email);
        if (user == null) return response;

        IdentityResult lockDate = await this._userManager.SetLockoutEndDateAsync(user, SystemDefault.LockOutEndDate);

        response.IsSuccess = lockDate.Succeeded;
        response.Result = lockDate.Succeeded;
        response.Message = "User locked out successfully";
        this._logger.LogInformation("User lock out processed with this response - {0}", lockDate.Succeeded);
        return response;
    }

    public async Task<ResponseDto<bool>> UnlockUser(string email)
    {
        var response = new ResponseDto<bool> { IsSuccess = false, Result = false, Message = "An error occurred whilst unlocking the user" };
        this._logger.LogInformation("Request to unlock user with this email -  {0}", email);
        ApplicationUser? user = await this._userManager.FindByEmailAsync(email);
        if (user == null) return response;

        IdentityResult setLockoutEndDate = await this._userManager.SetLockoutEndDateAsync(user, DateTime.Now - TimeSpan.FromMinutes(1));

        response.IsSuccess = setLockoutEndDate.Succeeded;
        response.Result = setLockoutEndDate.Succeeded;
        response.Message = "User unlocked successfully";
        this._logger.LogInformation("User unlock processed with this response - {0}", setLockoutEndDate.Succeeded);

        return response;

    }
}