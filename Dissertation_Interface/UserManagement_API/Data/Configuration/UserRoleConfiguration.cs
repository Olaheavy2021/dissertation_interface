using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UserManagement_API.Data.Configuration;
[ExcludeFromCodeCoverage]
public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder) =>
        builder.HasData(
            new IdentityUserRole<string>
            {
                RoleId = "059c8e03-ea50-4cbd-970f-5d9aea402cf6",
                UserId = "8e445865-a24d-4543-a6c6-9443d048cdb9"
            },
            new IdentityUserRole<string>
            {
                RoleId = "059c8e03-ea50-4cbd-970f-5d9aea402cf6",
                UserId = "9e224968-33e4-4652-b7b7-8574d048cdb9"
            }
        );
}