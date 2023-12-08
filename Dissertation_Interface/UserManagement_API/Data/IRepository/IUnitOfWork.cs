namespace UserManagement_API.Data.IRepository;

public interface IUnitOfWork
{
    IApplicationUserRepository ApplicationUserRepository { get; }
    ISupervisionCohortRepository SupervisionCohortRepository { get; }
    Task BeginTransactionAsync();
    Task SaveAsync(CancellationToken cancellationToken);
    Task CommitAsync();
    Task RollbackAsync();
}