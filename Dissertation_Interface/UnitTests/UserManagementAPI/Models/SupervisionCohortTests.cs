using UserManagement_API.Data.Models;

namespace UnitTests.UserManagementAPI.Models;

[TestFixture]
public class SupervisionCohortTests
{
    private const string SupervisorId = "supervisor123";
    private const int SupervisionSlot = 5;
    private const long DissertationCohortId = 1;

    [Test]
    public void Create_ShouldInitializePropertiesCorrectly()
    {
        var supervisionCohort = SupervisionCohort.Create(SupervisorId, SupervisionSlot, DissertationCohortId);
        Assert.Multiple(() =>
        {
            Assert.That(supervisionCohort.SupervisorId, Is.EqualTo(SupervisorId));
            Assert.That(supervisionCohort.SupervisionSlot, Is.EqualTo(SupervisionSlot));
            Assert.That(supervisionCohort.DissertationCohortId, Is.EqualTo(DissertationCohortId));
            Assert.That(supervisionCohort.AvailableSupervisionSlot, Is.EqualTo(SupervisionSlot));
        });
    }
}