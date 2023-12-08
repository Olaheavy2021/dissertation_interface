namespace UserManagement_API.Data.DomainHelper;

public interface IEntity<TId> : IEntity
{
    public TId Id { get; set; }
}

public interface IEntity
{

}