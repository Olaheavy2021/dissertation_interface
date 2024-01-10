using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagement_API.Data.Models;

namespace UserManagement_API.Data.Configuration;

[ExcludeFromCodeCoverage]
public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        var hasher = new PasswordHasher<ApplicationUser>();
        builder.HasData(
            new ApplicationUser
            {
                Id = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                Email = "superadmin1@shu.com",
                NormalizedEmail = "SUPERADMIN1@SHU.COM",
                FirstName = "Super",
                LastName = "Admin1",
                UserName = "superadmin1",
                NormalizedUserName = "SUPERADMIN1",
                PasswordHash = hasher.HashPassword(null, "Password10$"),
                EmailConfirmed = true
            },
            new ApplicationUser
            {
                Id = "9e224968-33e4-4652-b7b7-8574d048cdb9",
                Email = "superadmin2@shu.com",
                NormalizedEmail = "SUPERADMIN2@SHU.COM",
                FirstName = "Super",
                LastName = "Admin2",
                UserName = "superadmin2",
                NormalizedUserName = "SUPERADMIN2",
                PasswordHash = hasher.HashPassword(null, "Password10$"),
                EmailConfirmed = true
            }
        );
    }
}