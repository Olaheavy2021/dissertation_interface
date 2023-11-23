using Dissertation.Domain.DomainHelper;

namespace Dissertation.Domain.Entities;

public class Supervisor : AuditableEntity<long>
{
    public string Department { get; set; } = default!;

    public int StudentAllocation { get; set; }

    public string ProfilePicture { get; set; } = default!;

    public string UserId { get; set; } = default!;
}