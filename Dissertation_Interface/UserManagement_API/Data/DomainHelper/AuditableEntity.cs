using System.Diagnostics.CodeAnalysis;

namespace UserManagement_API.Data.DomainHelper;

[ExcludeFromCodeCoverage]
public class AuditableEntity<TId> : IAuditableEntity<TId>
{
    private DateTime? _createdDate;

    public TId Id { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedAt
    {
        get => this._createdDate ?? DateTime.UtcNow;
        set => this._createdDate = value;
    }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
}