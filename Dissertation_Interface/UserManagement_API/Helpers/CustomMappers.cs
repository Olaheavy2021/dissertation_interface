using Shared.DTO;
using Shared.Enums;
using UserManagement_API.Data.Models;

namespace UserManagement_API.Helpers;

public static class CustomMappers
{
    public static UserListDto MapToUserDto(ApplicationUser applicationUser) =>
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
}