using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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
        ApplicationUser user = new()
        {
            UserName = registrationRequestDto.UserName,
            Email = registrationRequestDto.Email,
            NormalizedEmail = registrationRequestDto.Email.ToUpper(),
            FirstName = registrationRequestDto.FirstName,
            LastName = registrationRequestDto.LastName,
            NormalizedUserName = registrationRequestDto.UserName.ToUpper()
        };

        try
        {
            IdentityResult result =await this._userManager.CreateAsync(user,registrationRequestDto.Password);
            if (result.Succeeded)
            {
                ApplicationUser? userToReturn = await this._db.ApplicationUserRepository.GetAsync(u => u.UserName == registrationRequestDto.Email);

                if (userToReturn is { Email: { }, UserName: { } })
                {
                    UserDto userDto = new()
                    {
                        Email = userToReturn.Email,
                        Id = userToReturn.Id,
                        FirstName = userToReturn.FirstName,
                        LastName = userToReturn.LastName,
                        UserName = userToReturn.UserName,
                    };

                    this._response.Message = "User Registration was Successful.";
                    this._response.IsSuccess = true;
                    this._response.Result = userDto;
                }
            }
            else
            {
                this._response.Message = result.Errors.FirstOrDefault()?.Description;
                this._response.IsSuccess = false;
                this._response.Result = registrationRequestDto;
            }

        }
        catch (Exception ex)
        {
            this._response.Message = "An unexpected error occurred. Please contact admin";
            this._response.IsSuccess = false;
            this._response.Result = registrationRequestDto;
            this._logger.LogError(ex.Message);
        }

        return this._response;
    }

    public Task<ResponseDto> RegisterStudentOrSupervisor(RegistrationRequestDto registrationRequestDto) => throw new NotImplementedException();

    public Task<ResponseDto> RegisterAdmin(RegistrationRequestDto registrationRequestDto) => throw new NotImplementedException();

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
                u.UserName.ToLower() == loginRequestDto.UserName.ToLower());

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
        /*catch (Exception ex)
        {
            this._logger.LogError(ex.Message, ex);
            this._response.IsSuccess = false;
            this._response.Message = "An unexpected error occurred. Please contact admin";
            return this._response;
        }*/
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

    private Task<bool> AssignRole(string email, string roleName) => throw new NotImplementedException();
}