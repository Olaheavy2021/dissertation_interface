using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Constants;
using Shared.Exceptions;
using Shared.Logging;
using Shared.MessageBus;
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
    private readonly IAppLogger<AuthService> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtSettings _jwtSettings;
    private readonly ServiceBusSettings _serviceBusSettings;
    private readonly ApplicationUrlSettings _applicationUrlSettings;
    private readonly IMapper _mapper;
    private readonly IMessageBus _messageBus;



    public AuthService(IUnitOfWork db, IOptions<ApplicationUrlSettings> applicationUrlSettings, IMessageBus messageBus,
        IOptions<JwtSettings> jwtSettings, SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager, IAppLogger<AuthService> logger, IMapper mapper, IOptions<ServiceBusSettings> serviceBusSettings)
    {
        this._db = db;
        this._userManager = userManager;
        this._signInManager = signInManager;
        this._logger = logger;
        this._jwtSettings = jwtSettings.Value;
        this._applicationUrlSettings = applicationUrlSettings.Value;
        this._serviceBusSettings = serviceBusSettings.Value;
        this._mapper = mapper;
        this._signInManager = signInManager;
        this._messageBus = messageBus;
    }

    private async Task<ResponseDto<string>> Register(RegistrationRequestDto registrationRequestDto)
    {
        //validate the incoming request
        ResponseDto<string> response = new();
        this._logger.LogInformation("Processing register request {@RegistrationRequestDto}", registrationRequestDto);
        var validator = new RegisterRequestDtoValidator(this._db);
        ValidationResult? validationResult = await validator.ValidateAsync(registrationRequestDto);
        if (validationResult.Errors.Any())
        {
            this._logger.LogWarning("Register - Validation errors in user registration for {0}", registrationRequestDto.Email);
            throw new BadRequestException("Invalid Registration Request", validationResult);
        }

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
        IdentityResult registrationResult = await this._userManager.CreateAsync(user, registrationRequestDto.Password);
        if (registrationResult.Succeeded)
        {
            //assign a role to the user
            IdentityResult roleResult = await AssignRole(user, registrationRequestDto.Role);

            if (roleResult.Succeeded)
            {
                if (registrationRequestDto.Role.ToLower().Equals(Roles.RoleAdmin) ||
                    registrationRequestDto.Role.ToLower().Equals(Roles.RoleSuperAdmin))
                    await PublishEmailConfirmationForAdminUsers(user);

                response.Message = "User Registration was Successful.";
                response.IsSuccess = true;
                response.Result = SuccessMessages.DefaultSuccess;
                this._logger.LogInformation("Register - User with this email registered successfully {0}", registrationRequestDto.Email);
            }
            else
            {
                response.Message = roleResult.Errors.FirstOrDefault()?.Description;
                response.IsSuccess = false;
                response.Result = ErrorMessages.DefaultError;

                this._logger.LogWarning("Register - An error occurred while registering this user {0} - {1}", registrationRequestDto.Email, roleResult.Errors.FirstOrDefault()?.Description ?? "Undefined Error");
            }
        }

        return response;
    }

    public Task<ResponseDto<RegistrationRequestDto>> RegisterStudentOrSupervisor(RegistrationRequestDto registrationRequestDto) => throw new NotImplementedException();

    public async Task<ResponseDto<string>> RegisterAdmin(AdminRegistrationRequestDto registrationRequestDto)
    {
        this._logger.LogInformation("Processing admin register request {@AdminRegistrationRequestDto}", registrationRequestDto);

        //validate the incoming request
        var validator = new AdminRegistrationRequestDtoValidator();
        ValidationResult? validationResult = await validator.ValidateAsync(registrationRequestDto);
        if (validationResult.Errors.Any())
        {
            this._logger.LogWarning("Register Admin - Validation errors while registering admin user - {0} ", registrationRequestDto.Email);
            throw new BadRequestException("Invalid Registration Request", validationResult);
        }

        var registrationRequest = new RegistrationRequestDto()
        {
            Password = SystemDefault.DefaultPassword,
            FirstName = registrationRequestDto.FirstName,
            LastName = registrationRequestDto.LastName,
            UserName = registrationRequestDto.UserName,
            Email = registrationRequestDto.Email,
            Role = registrationRequestDto.Role.ToLower()
        };

        return await Register(registrationRequest);
    }

    public async Task<ResponseDto<AuthResponseDto>> Login(LoginRequestDto loginRequestDto)
    {
        this._logger.LogInformation("Processing login request {@LoginRequestDto}", loginRequestDto);
        ResponseDto<AuthResponseDto> response = new() { IsSuccess = false, Message = "Invalid Login Attempt." };

        //validate the request
        var validator = new LoginRequestDtoValidator();
        ValidationResult? validationResult = await validator.ValidateAsync(loginRequestDto);
        if (validationResult.Errors.Any())
        {
            this._logger.LogWarning("Login - Validation errors in admin user registration for {0} - {1}", nameof(ApplicationUser), loginRequestDto.UserName);
            throw new BadRequestException("Invalid Login Request", validationResult);
        }

        //check if the password is correct and sign the user in
        SignInResult result =
            await this._signInManager.PasswordSignInAsync(loginRequestDto.UserName, loginRequestDto.Password, false,
                    true);
        if (result.Succeeded)
        {
            ApplicationUser? user = await this._userManager.FindByNameAsync(loginRequestDto.UserName);
            if (user == null)
            {
                this._logger.LogWarning("Login - User not found", loginRequestDto.UserName);
                return response;
            }

            IList<string> roles = await this._userManager.GetRolesAsync(user);
            //check if user is an admin and is signing in with the default password
            if ((roles.FirstOrDefault()!.ToLower().Equals(Roles.RoleAdmin) || roles.FirstOrDefault()!.ToLower().Equals(Roles.RoleSuperAdmin)) && loginRequestDto.Password.Equals(SystemDefault.DefaultPassword))
            {
                response.IsSuccess = false;
                response.Message = "You cannot sign in with the default password. Please reset your password";
                return response;
            }

            //generate jwt token and refresh token
            JwtSecurityToken jwtSecurityToken = await GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(this._jwtSettings.RefreshTokenValidityInDays);
            await this._userManager.UpdateAsync(user);

            //return the appropriate response
            UserDto? userToReturn = this._mapper.Map<UserDto>(user);
            var responseDto = new AuthResponseDto
            {
                User = userToReturn,
                AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Role = roles,
                RefreshToken = newRefreshToken
            };

            response.IsSuccess = true;
            response.Message = "Login Successful";
            response.Result = responseDto;
            this._logger.LogInformation("Login - User signed in successfully {0}", loginRequestDto.UserName);
            return response;
        }
        //check if the email is confirmed
        if (result.IsNotAllowed)
        {
            response.IsSuccess = false;
            response.Message = "Please confirm your email address, then try and login again.";
            this._logger.LogInformation("Login - Email has not been confirmed {0}", loginRequestDto.UserName);
            return response;
        }

        //check if the account is locked out
        if (result.IsLockedOut)
        {
            response.IsSuccess = false;
            response.Message = "Your account is currently locked out. Please contact admin or reset your password.";
            this._logger.LogInformation("Login - Account has been locked out {0}", loginRequestDto.UserName);
            return response;
        }

        return response;
    }

    public async Task<ResponseDto<string>> InitiatePasswordReset(InitiatePasswordResetDto request)
    {
        this._logger.LogInformation("Initiating password request {@InitiatePasswordResetDto}", request);
        ResponseDto<string> response = new() { IsSuccess = true, Message = "A notification will be sent to this email if an account is registered under it." };

        ApplicationUser? user = await this._userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return response;

        //check if the user is not locked out by the admin
        if (user.IsLockedOutByAdmin)
        {
            response.IsSuccess = false;
            response.Message = "You account has been deactivated. Please contact admin..";
            return response;
        }

        //check if the account is system default account
        if (user.UserName != null && (user.UserName.Equals(SystemDefault.DefaultSuperAdmin1) ||
                                      user.UserName.Equals(SystemDefault.DefaultSuperAdmin2)))
        {
            this._logger.LogWarning("Password reset initiated for a default user {0}", request.Email);
            response.Message = "You cannot initiate password for a system default user";
            response.IsSuccess = false;
            return response;
        }

        await PublishPasswordResetEmail(user);
        return response;
    }

    public async Task<ResponseDto<string>> ConfirmPasswordReset(ConfirmPasswordResetDto request)
    {
        ResponseDto<string> response = new();
        this._logger.LogInformation("Confirm password request {@ConfirmPasswordResetDto}", request);
        response.Message = "Invalid Password Reset Request";
        response.IsSuccess = false;

        ApplicationUser? user = await this._userManager.FindByNameAsync(request.UserName);
        if (user == null)
            return response;

        IdentityResult result = await this._userManager.ResetPasswordAsync(user, request.Token, request.Password);
        if (result.Succeeded)
        {
            this._logger.LogInformation("Password reset completed for this user {0}", request.UserName);
            response.Message = "Your password has been reset. Please sign in.";
            response.IsSuccess = true;
            return response;
        }

        response.Message = result.Errors.FirstOrDefault()?.Description;
        this._logger.LogWarning("Password reset failed with an error {0} - {1}", request.UserName, response.Message ?? "Undefined identity error");
        return response;
    }

    public async Task<ResponseDto<ConfirmEmailResponseDto>> ConfirmEmail(ConfirmEmailRequestDto request)
    {
        ResponseDto<ConfirmEmailResponseDto> response = new();
        this._logger.LogInformation("Confirm email request {@ConfirmEmailRequestDto}", request);
        response.Message = "Invalid Email Confirmation Request";
        response.IsSuccess = false;

        ApplicationUser? user = await this._userManager.FindByNameAsync(request.UserName);
        if (user == null)
            return response;

        var isEmailConfirmed = await this._userManager.IsEmailConfirmedAsync(user);
        if (isEmailConfirmed)
        {
            response.Message = "The email for this account has been confirmed already. Please sign in";
            response.IsSuccess = false;

            return response;
        }

        IdentityResult result = await this._userManager.ConfirmEmailAsync(user, request.Token);
        if (result.Succeeded)
        {
            this._logger.LogInformation("Email has been confirmed for this user {0}", request.UserName);
            response.Message = "Your email has been confirmed. Please sign in.";
            response.IsSuccess = true;
            UserDto? userToReturn = this._mapper.Map<UserDto>(user);
            IList<string> roles = await this._userManager.GetRolesAsync(user);
            var responseDto = new ConfirmEmailResponseDto
            {
                User = userToReturn,
                Role = roles.FirstOrDefault() ?? string.Empty
            };

            //generate a reset password token if the user is an admin or superadmin
            if (responseDto.Role.ToLower().Equals(Roles.RoleAdmin) || responseDto.Role.ToLower().Equals(Roles.RoleSuperAdmin))
            {
                var code = await this._userManager.GeneratePasswordResetTokenAsync(user);
                responseDto.PasswordResetToken = code;
            }
            response.Result = responseDto;
            return response;
        }

        response.Message = result.Errors.FirstOrDefault()?.Description;
        this._logger.LogInformation("Email confirmation failed for this user {0} - {1}", request.UserName, response.Message ?? "Undefined Identity Error");
        return response;
    }

    public async Task<ResponseDto<string>> ResendConfirmationEmail(EmailRequestDto request, string? loggedInAdminEmail)
    {
        if (string.IsNullOrEmpty(loggedInAdminEmail))
            throw new UnauthorizedException();

        ResponseDto<string> response = new() { IsSuccess = false, Message = "Invalid Request", Result = ErrorMessages.DefaultError };
        ApplicationUser? user = await this._userManager.FindByEmailAsync(request.Email) ?? throw new NotFoundException(nameof(ApplicationUser), request.Email);
        if (user.EmailConfirmed)
        {
            response.Message = "The email for this user has been confirmed already";
            response.Result = ErrorMessages.DefaultError;
            await this._messageBus.PublishAuditLog(EventType.ResendEmailConfirmation,
                this._serviceBusSettings.ServiceBusConnectionString, loggedInAdminEmail, ErrorMessages.DefaultError);
            return response;
        }

        response.IsSuccess = true;
        response.Message = "Confirmation Email has been resent to the user";
        response.Result = SuccessMessages.DefaultSuccess;
        await PublishEmailConfirmationForAdminUsers(user);
        await this._messageBus.PublishAuditLog(EventType.ResendEmailConfirmation,
            this._serviceBusSettings.ServiceBusConnectionString, loggedInAdminEmail, SuccessMessages.DefaultSuccess);
        return response;
    }

    public async Task<ResponseDto<RefreshTokenDto>> GetRefreshToken(RefreshTokenDto request)
    {
        ResponseDto<RefreshTokenDto> response = new() { IsSuccess = false, Message = "Invalid access token or refresh token" };
        ClaimsPrincipal principal = GetPrincipalFromExpiredToken(request.AccessToken);
        var email = principal.Claims.First(c => c.Type == ClaimTypes.Email).Value;

        //check that the username exists on the token
        if (string.IsNullOrEmpty(email)) return response;

        ApplicationUser? user = await this._userManager.FindByEmailAsync(email);

        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            response.IsSuccess = false;
            response.Message = "Invalid access token or refresh token";
            return response;
        }

        //generate jwt token
        JwtSecurityToken jwtSecurityToken = await GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(this._jwtSettings.RefreshTokenValidityInDays);
        await this._userManager.UpdateAsync(user);

        response.IsSuccess = true;
        response.Message = "Success";
        response.Result = new RefreshTokenDto
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            RefreshToken = newRefreshToken
        };

        return response;

    }

    private async Task<JwtSecurityToken> GenerateJwtToken(ApplicationUser user)
    {
        IList<Claim> userClaims = await this._userManager.GetClaimsAsync(user);
        IList<string> roles = await this._userManager.GetRolesAsync(user);

        var roleClaims = roles.Select(q => new Claim(ClaimTypes.Role, q)).ToList();

        if (user is { UserName: not null, Email: not null })
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

    private async Task PublishEmailConfirmationForAdminUsers(ApplicationUser user)
    {
        var code = await this._userManager.GenerateEmailConfirmationTokenAsync(user);

        var callbackUrl = $"{this._applicationUrlSettings.WebClientUrl}/{this._applicationUrlSettings.WebConfirmEmailRoute}?username={user.UserName}&activationToken={code}";
        UserDto? userToReturn = this._mapper.Map<UserDto>(user);
        this._logger.LogInformation("Email Confirmation Published for this user {0}", user.UserName ?? string.Empty);


        var emailDto = new PublishEmailDto { User = userToReturn, CallbackUrl = callbackUrl, EmailType = EmailType.EmailTypeAdminConfirmationEmail };
        await this._messageBus.PublishMessage(emailDto, this._serviceBusSettings.EmailLoggerQueue,
            this._serviceBusSettings.ServiceBusConnectionString);
    }
    private async Task PublishPasswordResetEmail(ApplicationUser user)
    {
        var code = await this._userManager.GeneratePasswordResetTokenAsync(user);
        var callbackUrl = $"{this._applicationUrlSettings.WebClientUrl}/{this._applicationUrlSettings.WebResetPasswordRoute}?username={user.UserName}&activationToken={code}";
        UserDto? userToReturn = this._mapper.Map<UserDto>(user);
        this._logger.LogInformation("Password Reset Email Published for this user {0}", user.UserName ?? string.Empty);


        var emailDto = new PublishEmailDto { User = userToReturn, CallbackUrl = callbackUrl, EmailType = EmailType.EmailTypeResetPasswordEmail };
        await this._messageBus.PublishMessage(emailDto, this._serviceBusSettings.EmailLoggerQueue,
            this._serviceBusSettings.ServiceBusConnectionString);
    }
    private ClaimsPrincipal GetPrincipalFromExpiredToken(string? token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._jwtSettings.Secret)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        ClaimsPrincipal? principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}