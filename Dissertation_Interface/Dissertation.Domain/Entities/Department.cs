using Dissertation.Domain.DomainHelper;
using Dissertation.Domain.Enums;

namespace Dissertation.Domain.Entities;

public class Department : AuditableEntity<long>
{
    public string Name { get; set; }

    public DissertationConfigStatus Status { get; set; }

    private Department(
        string name)
    {
        Name = name;
        Status = DissertationConfigStatus.Active;
    }

    public static Department Create(string name) =>
        new(name);
}