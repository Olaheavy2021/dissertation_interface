using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.UserManagementAPI.AuthService;

public class FakeRoleManager : RoleManager<IdentityRole>
{
    public FakeRoleManager()
        : base(
            new Mock<IRoleStore<IdentityRole>>().Object, Array.Empty<IRoleValidator<IdentityRole>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<ILogger<RoleManager<IdentityRole>>>().Object
            )
    { }

}