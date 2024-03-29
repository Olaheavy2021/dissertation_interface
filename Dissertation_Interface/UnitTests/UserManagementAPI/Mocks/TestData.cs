using System.Security.Claims;
using Dissertation.Domain.Entities;
using Notification_API.Settings;
using Shared.DTO;
using Shared.Settings;
using UserManagement_API.Data.Models;
using UserManagement_API.Data.Models.Dto;

namespace UnitTests.UserManagementAPI.Mocks;

public static class TestData
{
    private const string UserName = "unittest";
    private const string FirstName = "Unit";
    private const string LastName = "Test";
    private const string UserId = "05d73022-a466-496d-89e5-1a13e70106f2";
    private const string Email = "unittest@hallam.shu.ac.uk";
    internal static readonly LoginRequestDto LoginRequest = new() { Email = Email, Password = "Unittest10$" };

    internal static readonly ApplicationUser User = new()
    {
        UserName = UserName,
        FirstName = FirstName,
        LastName = LastName,
        Email = Email,
        Id = UserId,
        IsLockedOutByAdmin = false
    };

    internal static readonly ProfilePicture ProfilePicture = ProfilePicture.Create(
        "jhahaj",
        "abndbnadbn",
        "jdshjshd",
        UserId
    );

    internal static readonly JwtSettings JwtSettings = new()
    {
        Audience = "SHUDissertationInterfaceUser",
        DurationInMinutes = 60,
        Secret = "SECRET_JWT_KEY_HERE",
        Issuer = "SHU_Dissertation_Interface.Api",
        RefreshTokenValidityInDays = 7
    };

    internal static readonly ApplicationUrlSettings ApplicationUrlSettings = new()
    {
        WebConfirmEmailRoute = "confirm-email",
        WebResetPasswordRoute = "change-password",
        WebClientUrl = "https://localhost:3000"
    };

    internal static readonly ServiceBusSettings ServiceBusSettings = new()
    {
        ServiceBusConnectionString = "Endpoint=sb",
        AuditLoggerQueue = "auditlogger",
        EmailLoggerQueue = "emaillogger"
    };

    internal static readonly SendGridSettings SendGridSettings = new()
    {
        ApiKey = "gdgdgddgdgdg",
        From = "c2042523@hallam.shu.ac.uk",
        Name = "SHU Dissertation Interface",
        TestEmail = "dissertationinterfacetest@gmail.com",
        AdminEmail = "dissertationinterfacetest@gmail.com"
    };

    internal static readonly UserDto UserDtoResponse = new()
    {
        UserName = UserName,
        FirstName = FirstName,
        LastName = LastName,
        Id = UserId,
    };

    internal static readonly IList<Claim> UserClaims = new List<Claim>
    {
        new("name", "unittest")
    };

    internal static readonly InitiatePasswordResetDto InitiatePasswordResetRequest = new() { Email = Email };

    internal static readonly ConfirmPasswordResetDto ConfirmPasswordRequest =
        new() { UserName = UserName, Token = "token", Password = "password" };

    internal static readonly ConfirmEmailRequestDto ConfirmEmailRequest = new()
    {
        UserName = UserName,
        Token = "token"
    };

    internal static readonly EmailRequestDto EmailRequestDto = new() { Email = Email };

    internal static readonly AdminRegistrationRequestDto AdminRegistrationRequestDto = new()
    {
        Email = Email,
        UserName = UserName,
        FirstName = FirstName,
        LastName = LastName,
        Role = "Admin"
    };

    internal static readonly StudentOrSupervisorRegistrationDto StudentOrSupervisorRegistrationDto = new()
    {
        Password = "Password",
        FirstName = FirstName,
        LastName = LastName,
        UserName = UserName,
        CourseId = 1,
        DepartmentId = 2
    };

    internal static readonly Supervisor Supervisor = Supervisor.Create(User.Id, 1);
}