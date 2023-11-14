using Dissertation.Domain.DomainHelper;
using Dissertation.Domain.Enums;

namespace Dissertation.Domain.Entities;

public class Department : AuditableEntity<long>
{
    public string Name { get; set; }

    public DissertationConfigStatus Status { get; set; }

    private Department(
        string name,
        DissertationConfigStatus status)
    {
        Name = name;
        Status = status;
    }

    public static Department Create(string name, DissertationConfigStatus status) =>
        new(name, status);
}