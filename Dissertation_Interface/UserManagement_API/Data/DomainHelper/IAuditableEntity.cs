using System.Diagnostics.CodeAnalysis;

namespace UserManagement_API.Data.DomainHelper;


public interface IAuditableEntity<TId> : IAuditableEntity, IEntity<TId>
{

}
public interface IAuditableEntity : IEntity
{
    string CreatedBy { get; set; }

    DateTime CreatedAt { get; set; }

    string UpdatedBy { get; set; }

    DateTime? UpdatedAt { get; set; }
}