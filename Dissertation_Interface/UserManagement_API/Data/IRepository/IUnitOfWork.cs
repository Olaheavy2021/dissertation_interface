namespace UserManagement_API.Data.IRepository;

public interface IUnitOfWork
{
    IApplicationUserRepository ApplicationUserRepository { get; }
    ISupervisionCohortRepository SupervisionCohortRepository { get; }
    ISupervisionRequestRepository SupervisionRequestRepository { get; }
    Task BeginTransactionAsync();
    Task SaveAsync(CancellationToken cancellationToken);
    Task CommitAsync();
    Task RollbackAsync();
}