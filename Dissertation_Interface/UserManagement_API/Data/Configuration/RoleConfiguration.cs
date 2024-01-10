using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UserManagement_API.Data.Configuration;
[ExcludeFromCodeCoverage]
public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder) =>
        builder.HasData(
            new IdentityRole
            {
                Id = "059c8e03-ea50-4cbd-970f-5d9aea402cf6",
                Name = "Superadmin",
                NormalizedName = "SUPERADMIN"
            },
            new IdentityRole
            {
                Id = "4846bb17-dffe-4158-b387-1711112ad6fd",
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole
            {
                Id = "0145dfef-6553-4093-a350-c8e53fc4933b",
                Name = "Student",
                NormalizedName = "STUDENT"
            },
            new IdentityRole
            {
                Id = "3df21c54-7f4f-4b50-95e4-3904a1e00f0c",
                Name = "Supervisor",
                NormalizedName = "SUPERVISOR"
            }
        );
}