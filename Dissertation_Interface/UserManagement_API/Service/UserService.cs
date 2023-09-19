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

    public UserService(IUnitOfWork db,IAppLogger<UserService> logger, IMapper mapper, UserManager<ApplicationUser> userManager)
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

        IList<ApplicationUser> adminUsers =await this._userManager.GetUsersInRoleAsync(RolesEnum.AdminRoles.Admin.ToString());
        if(adminUsers.Any())
            users.AddRange(adminUsers);
        IList<ApplicationUser> superAdminUsers =await this._userManager.GetUsersInRoleAsync(RolesEnum.AdminRoles.Superadmin.ToString());
        if(superAdminUsers.Any())
            users.AddRange(adminUsers);

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

        return response;
    }

    public async Task<ResponseDto<GetUserDto>> GetUser(string userId)
    {
        var response = new ResponseDto<GetUserDto>();
        ApplicationUser? user = await this._db.ApplicationUserRepository.GetFirstOrDefaultAsync(a => a.Id == userId);
        if (user != null)
        {
            IList<string> roles = await this._userManager.GetRolesAsync(user);
            var isLockedOut = await this._userManager.IsLockedOutAsync(user);
            UserDto mappedUser = this._mapper.Map<ApplicationUser, UserDto>(user);
            var getUserDto = new GetUserDto() { User = mappedUser, Role = roles.FirstOrDefault() ?? string.Empty, IsLockedOut = isLockedOut};

            response.IsSuccess = true;
            response.Message = SuccessMessages.DefaultSuccess;
            response.Result = getUserDto;
            return response;
        }

        response.IsSuccess = false;
        response.Message = "No user found";
        return response;
    }
}