using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using Shared.Constants;
using Shared.Exceptions;
using Shared.Extensions;
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
    private readonly IAppLogger<AuthService> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ResponseDto _response;
    private readonly JwtSettings _jwtSettings;
    private readonly ApplicationUrlSettings _applicationUrlSettings;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;
    private readonly string _folderPath;


    public AuthService(IUnitOfWork db, IOptions<ApplicationUrlSettings> applicationUrlSettings,
        IOptions<JwtSettings> jwtSettings, SignInManager<ApplicationUser> signInManager,IWebHostEnvironment env,
        UserManager<ApplicationUser> userManager, IAppLogger<AuthService> logger, IMapper mapper)
    {
        this._db = db;
        this._userManager = userManager;
        this._signInManager = signInManager;
        this._logger = logger;
        this._jwtSettings = jwtSettings.Value;
        this._applicationUrlSettings = applicationUrlSettings.Value;
        this._mapper = mapper;
        this._signInManager = signInManager;
        this._response = new();
        this._env = env;
        this._folderPath = Path.Combine( this._env.WebRootPath, "/Email/Snapshots");
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
        //validate the incoming request
        var validator = new AdminRegistrationRequestDtoValidator();
        ValidationResult? validationResult = await validator.ValidateAsync(registrationRequestDto);
        if (validationResult.Errors.Any())
            throw new BadRequestException("Invalid Registration Request", validationResult);

        var registrationRequest = new RegistrationRequestDto()
        {
            Password = SystemDefault.DefaultPassword,
            FirstName = registrationRequestDto.FirstName,
            LastName = registrationRequestDto.LastName,
            UserName = registrationRequestDto.UserName,
            Email = registrationRequestDto.Email,
            Role = registrationRequestDto.Role.ToLower()
        };

        // TODO: initiate an Event to send an email.

       return await Register(registrationRequest);
    }

    public async Task<ResponseDto> Login(LoginRequestDto loginRequestDto)
    {
        this._response.IsSuccess = false;
        this._response.Message = "Invalid Login Attempt.";

        //validate the request
        var validator = new LoginRequestDtoValidator();
        ValidationResult? validationResult = await validator.ValidateAsync(loginRequestDto);
        if (validationResult.Errors.Any())
            throw new BadRequestException("Invalid Login Request", validationResult);


        //check if the password is correct and sign the user in
        SignInResult result =
            await this._signInManager.PasswordSignInAsync(loginRequestDto.UserName, loginRequestDto.Password, false,
                    true);
        if (result.Succeeded)
        {
            ApplicationUser? user = await this._db.ApplicationUserRepository.GetFirstOrDefaultAsync(u =>
               u.UserName != null && u.NormalizedUserName == loginRequestDto.UserName.ToUpper());
            if (user == null)
            {
                return this._response;
            }
            //generate jwt token
            JwtSecurityToken jwtSecurityToken = await GenerateJwtToken(user);

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
        //check if the email is confirmed
        if (result.IsNotAllowed)
        {
            this._response.IsSuccess = false;
            this._response.Message = "Please confirm your email address, then try and login again.";
            return this._response;
        }
        //check if the account is locked out
        if (result.IsLockedOut)
        {
            this._response.IsSuccess = false;
            this._response.Message = "Your account is currently locked out";
            return this._response;
        }

        return this._response;
    }

    public async Task<ResponseDto> InitiatePasswordReset(InitiatePasswordResetDto request)
    {
        this._response.IsSuccess = true;
        this._response.Message = "A notification will be sent to this email if an account is registered under it.";
        ApplicationUser? user = await this._db.ApplicationUserRepository.GetAsync(x => x.NormalizedEmail == request.Email.ToUpper());
        if (user == null)
            return this._response;

        await GeneratePasswordResetLink(user);
        return this._response;
    }

    public async Task<ResponseDto> ConfirmPasswordReset(ConfirmPasswordResetDto request)
    {
        this._response.Message = "Invalid Password Reset Request";
        this._response.IsSuccess = false;

        ApplicationUser? user = await this._db.ApplicationUserRepository.GetFirstOrDefaultAsync(u =>
            u.UserName != null && u.NormalizedUserName == request.UserName.ToUpper());
        if (user == null)
            return this._response;

        IdentityResult result = await this._userManager.ResetPasswordAsync(user, Encoding.UTF8.DecodeBase64( request.Token), request.Password);
        if (result.Succeeded)
        {
            this._response.Message = "Your password has been reset. Please sign in.";
            this._response.IsSuccess = true;
            return this._response;
        }

        this._response.Message = result.Errors.FirstOrDefault()?.Description;
        return this._response;
    }

    public async Task<ResponseDto> ConfirmEmail(ConfirmEmailDto request)
    {
        this._response.Message = "Invalid Email Confirmation Request";
        this._response.IsSuccess = false;

        ApplicationUser? user = await this._db.ApplicationUserRepository.GetFirstOrDefaultAsync(u =>
            u.UserName != null && u.NormalizedUserName == request.UserName.ToUpper());
        if (user == null)
            return this._response;

        IdentityResult result = await this._userManager.ConfirmEmailAsync(user, Encoding.UTF8.DecodeBase64( request.Token));
        if (result.Succeeded)
        {
            this._response.Message = "Your email has been confirmed. Please sign in.";
            this._response.IsSuccess = true;
            return this._response;
        }

        this._response.Message = result.Errors.FirstOrDefault()?.Description;
        return this._response;
    }

    private async Task<JwtSecurityToken> GenerateJwtToken(ApplicationUser user)
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

    private async Task GeneratePasswordResetLink(ApplicationUser user)
    {
        var code = await this._userManager.GeneratePasswordResetTokenAsync(user);

        var callbackUrl = $"{this._applicationUrlSettings.WebClientUrl}/{this._applicationUrlSettings.WebResetPasswordRoute}?username={user.UserName}&activationToken={Encoding.UTF8.EncodeBase64(code)}";

        var webRoot = this._env.WebRootPath; //get wwwroot Folder

        //Get TemplateFile located at wwwroot/Templates/EmailTemplates/
        var pathToFile = this._env.WebRootPath
                         + Path.DirectorySeparatorChar
                         + "Templates"
                         + Path.DirectorySeparatorChar
                         + "EmailTemplates"
                         + Path.DirectorySeparatorChar
                         + "Change_Password.Html";

        var builder = new BodyBuilder();

        using StreamReader sourceReader = File.OpenText(pathToFile);
        builder.HtmlBody = await sourceReader.ReadToEndAsync();

        //{0} : Subject
        const string subject = "Reset Password";
        //{1} : Date
        //{2} : FirstName
        //{3} : Username
        //{4} : CallbackUrl

        string messageBody = string.Format(builder.HtmlBody,
            subject,
            String.Format("{0:dddd, d MMMM yyyy}", DateTime.Now),
            user.FirstName,
            user.UserName,
            callbackUrl
        );
        await SaveToFile("Reset_Email.html", messageBody);

    }

    private async Task GenerateEmailConfirmationLink(ApplicationUser user)
    {
        var code = await this._userManager.GenerateEmailConfirmationTokenAsync(user);

        var callbackUrl = $"{this._applicationUrlSettings.WebClientUrl}/{this._applicationUrlSettings.WebConfirmEmailRoute}?username={user.UserName}&activationToken={Encoding.UTF8.EncodeBase64(code)}";

        //Get TemplateFile located at wwwroot/Templates/EmailTemplates/
        var pathToFile = this._env.WebRootPath
                         + Path.DirectorySeparatorChar
                         + "Templates"
                         + Path.DirectorySeparatorChar
                         + "EmailTemplates"
                         + Path.DirectorySeparatorChar
                         + "Welcome_Email.Html";

        var builder = new BodyBuilder();

        using StreamReader sourceReader = File.OpenText(pathToFile);
        builder.HtmlBody = await sourceReader.ReadToEndAsync();

        //{0} : Subject
        const string subject = "Confirm Email";
        //{1} : Date
        //{2} : FirstName
        //{3} : Username
        //{4} : CallbackUrl

        string messageBody = string.Format(builder.HtmlBody,
            subject,
            String.Format("{0:dddd, d MMMM yyyy}", DateTime.Now),
            user.FirstName,
            user.UserName,
            callbackUrl
        );

        //Send email to the user
        await SaveToFile("Confirm_Email.html", messageBody);

    }

    private async Task SaveToFile(string name, string content)
    {
        var fullPath = Path.Combine(this._folderPath, name);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? string.Empty);
        await File.WriteAllTextAsync(fullPath, content);
    }
}