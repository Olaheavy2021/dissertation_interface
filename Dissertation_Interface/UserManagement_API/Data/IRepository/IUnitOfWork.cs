namespace UserManagement_API.Data.IRepository;

public interface IUnitOfWork
{
    IApplicationUserRepository ApplicationUserRepository { get; }
    Task BeginTransactionAsync();
    Task SaveAsync();
    Task CommitAsync();
    Task RollbackAsync();
}