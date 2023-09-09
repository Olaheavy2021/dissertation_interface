using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Constants;
using Shared.Exceptions;
using Shared.Logging;
using Shared.Settings;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;
using UserManagement_API.Data.Models.Dto;
using UserManagement_API.Data.Models.Validators;
using UserManagement_API.Service.IService;

namespace UserManagement_API.Service;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IAppLogger<AuthService> _logger;
    private readonly ResponseDto _response;
    private readonly JwtSettings _jwtSettings;
    private readonly IMapper _mapper;

    public AuthService(IUnitOfWork db,
        IOptions<JwtSettings> jwtSettings,
        UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IAppLogger<AuthService> logger, IMapper mapper)
    {
        this._db = db;
        this._userManager = userManager;
        this._roleManager = roleManager;
        this._logger = logger;
        this._jwtSettings = jwtSettings.Value;
        this._mapper = mapper;
        this._response = new();
    }

    private async Task<ResponseDto> Register(RegistrationRequestDto registrationRequestDto)
    {
        //validate the incoming request
        var validator = new RegisterRequestDtoValidator(this._db);
        ValidationResult? validationResult = await validator.ValidateAsync(registrationRequestDto);
        if (validationResult.Errors.Any())
            throw new BadRequestException("Invalid Registration Request", validationResult);

        // register the user
        ApplicationUser user = new()
            {
                UserName = registrationRequestDto.UserName,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                FirstName = registrationRequestDto.FirstName,
                LastName = registrationRequestDto.LastName,
                NormalizedUserName = registrationRequestDto.UserName.ToUpper()
            };
        IdentityResult registrationResult =await this._userManager.CreateAsync(user,registrationRequestDto.Password);
        if (registrationResult.Succeeded)
        {
            //assign a role to the user
            IdentityResult roleResult = await AssignRole(user, registrationRequestDto.Role);

            if (roleResult.Succeeded)
            {
                UserDto userDto = new()
                {
                    Email = user.Email,
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName
                };

                this._response.Message = "User Registration was Successful.";
                this._response.IsSuccess = true;
                this._response.Result = userDto;
            }
            else
            {
                this._response.Message = roleResult.Errors.FirstOrDefault()?.Description;
                this._response.IsSuccess = false;
                this._response.Result = registrationRequestDto;
            }
        }

        return this._response;
    }

    public Task<ResponseDto> RegisterStudentOrSupervisor(RegistrationRequestDto registrationRequestDto) => throw new NotImplementedException();

    public async Task<ResponseDto> RegisterAdmin(AdminRegistrationRequestDto registrationRequestDto)
    {
        var registrationRequest = new RegistrationRequestDto()
        {
            Password = "Password10$",
            FirstName = registrationRequestDto.FirstName,
            LastName = registrationRequestDto.LastName,
            UserName = registrationRequestDto.UserName,
            Email = registrationRequestDto.Email,
            Role = Roles.RoleAdmin
        };

       return await Register(registrationRequest);
    }

    public async Task<ResponseDto> Login(LoginRequestDto loginRequestDto)
    {
        try
        {
            var validator = new LoginRequestDtoValidator();
            ValidationResult? validationResult = await validator.ValidateAsync(loginRequestDto);

            if (validationResult.Errors.Any())
                throw new BadRequestException("Invalid Login Request", validationResult);

            //check the database for the user using the username
            ApplicationUser? user = await this._db.ApplicationUserRepository.GetFirstOrDefaultAsync(u =>
                u.UserName != null && u.NormalizedUserName == loginRequestDto.UserName.ToUpper());

            //check if the user exists
            if (user == null)
            {
                throw new NotFoundException(nameof(ApplicationUser), loginRequestDto.UserName);
            }

            //check if the password is correct
            var isValid = await this._userManager.CheckPasswordAsync(user, loginRequestDto.Password);

            if (isValid == false)
            {
                throw new NotFoundException(nameof(ApplicationUser), user.Id);
            }

            //check if the email is confirmed
            if (!user.EmailConfirmed)
            {
                this._response.IsSuccess = false;
                this._response.Message = "Please confirm your email before you can sign in.";
                return this._response;
            }

            //generate jwt token
            JwtSecurityToken jwtSecurityToken = await GenerateToken(user);

            //return the appropriate response
            UserDto? userToReturn = this._mapper.Map<UserDto>(user);
            var responseDto = new LoginResponseDto()
            {
                User = userToReturn, Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken)
            };

            this._response.IsSuccess = true;
            this._response.Message = "Login Successful";
            this._response.Result = responseDto;

            return this._response;
        }
        catch (NotFoundException ex)
        {
            this._logger.LogWarning("User details is incomplete or does not exist", ex);
            this._response.IsSuccess = false;
            this._response.Message = "Invalid username or password entered.";
            return this._response;
        }
    }

    private async Task<JwtSecurityToken> GenerateToken(ApplicationUser user)
    {
        IList<Claim> userClaims = await this._userManager.GetClaimsAsync(user);
        IList<string> roles = await this._userManager.GetRolesAsync(user);

        var roleClaims = roles.Select(q => new Claim(ClaimTypes.Role, q)).ToList();

        if (user.UserName != null && user.Email != null)
        {
            IEnumerable<Claim> claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("uid", user.Id)
                }
                .Union(userClaims)
                .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._jwtSettings.Secret));

            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: this._jwtSettings.Issuer,
                audience: this._jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(this._jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }

        throw new NotFoundException(nameof(ApplicationUser), user.Id);
    }

    private async Task<IdentityResult> AssignRole(ApplicationUser user, string roleName) => await this._userManager.AddToRoleAsync(user, roleName);
}