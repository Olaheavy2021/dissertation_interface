using System.Security.Claims;
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
    internal static readonly LoginRequestDto LoginRequest = new() { UserName = UserName, Password = "Unittest10$" };

    internal static readonly ApplicationUser User = new()
    {
        UserName = UserName,
        FirstName = FirstName,
        LastName = LastName,
        Email = Email,
        Id = UserId
    };

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
        AccountLockedOutQueue = "emailaccountlockedout",
        ServiceBusConnectionString = "Endpoint=sb",
        RegisterAdminUserQueue = "emailregisteradminuser",
        ResetPasswordQueue = "emailresetpassword",
    };

    internal static readonly UserDto UserDtoResponse = new()
    {
        UserName = UserName,
        FirstName = FirstName,
        LastName = LastName,
        Id = UserId
    };

    internal static readonly IList<Claim> UserClaims = new List<Claim>
    {
        new("name", "unittest")
    };

    internal static readonly InitiatePasswordResetDto InitiatePasswordResetRequest =
        new InitiatePasswordResetDto { Email = Email };

    internal static readonly ConfirmPasswordResetDto ConfirmPasswordRequest =
        new ConfirmPasswordResetDto { UserName = UserName, Token = "token", Password = "password" };
}