using UserManagement_API.Data.Models;

namespace UnitTests.UserManagementAPI.Models;

[TestFixture]
public class SupervisionListTests
{
    private const string SupervisorId = "supervisor123";
    private const string StudentId = "student456";
    private const long DissertationCohortId = 100;

    [Test]
    public void Create_ShouldInitializePropertiesCorrectly()
    {
        var supervisionList = SupervisionList.Create(SupervisorId, StudentId, DissertationCohortId);
        Assert.Multiple(() =>
        {
            Assert.That(supervisionList.SupervisorId, Is.EqualTo(SupervisorId));
            Assert.That(supervisionList.StudentId, Is.EqualTo(StudentId));
            Assert.That(supervisionList.DissertationCohortId, Is.EqualTo(DissertationCohortId));
        });
    }
}