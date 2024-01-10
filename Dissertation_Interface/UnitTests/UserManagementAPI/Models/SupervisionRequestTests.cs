using Shared.Enums;
using UserManagement_API.Data.Models;

namespace UnitTests.UserManagementAPI.Models;

[TestFixture]
public class SupervisionRequestTests
{
    private const string SupervisorId = "supervisor123";
    private const string StudentId = "student456";
    private const long DissertationCohortId = 100;

    [Test]
    public void Create_ShouldInitializePropertiesCorrectly()
    {
        var supervisionRequest = SupervisionRequest.Create(SupervisorId, StudentId, DissertationCohortId);
        Assert.Multiple(() =>
        {
            Assert.That(supervisionRequest.SupervisorId, Is.EqualTo(SupervisorId));
            Assert.That(supervisionRequest.StudentId, Is.EqualTo(StudentId));
            Assert.That(supervisionRequest.DissertationCohortId, Is.EqualTo(DissertationCohortId));
            Assert.That(supervisionRequest.Status, Is.EqualTo(SupervisionRequestStatus.Pending));
        });
        Assert.IsNull(supervisionRequest.Comment);
    }

    [Test]
    public void Comment_CanBeSetAndRetrieved()
    {
        var supervisionRequest = SupervisionRequest.Create(SupervisorId, StudentId, DissertationCohortId);
        const string testComment = "Test Comment";

        supervisionRequest.Comment = testComment;

        Assert.That(supervisionRequest.Comment, Is.EqualTo(testComment));
    }
}