using Microsoft.EntityFrameworkCore.Storage;
using UserManagement_API.Data.IRepository;

namespace UserManagement_API.Data.Repository;

public class UnitOfWork : IUnitOfWork
{
    private IDbContextTransaction _transaction;
    public UserDbContext Context;

    public UnitOfWork(UserDbContext context) => this.Context = context;

    #region Dispose
    private bool _disposedValue = false;
    ~UnitOfWork() => Dispose(disposing: false);

    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposedValue)
        {
            if (disposing && this.Context != null)
            {
                this.Context.Dispose();
            }

            this._disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion

    #region Repositories

    public IApplicationUserRepository ApplicationUserRepository => new ApplicationUserRepository(this.Context);

    #endregion Repositories

    public async Task BeginTransactionAsync() => this._transaction = await this.Context.Database.BeginTransactionAsync().ConfigureAwait(false);

    public async Task CommitAsync()
    {
        await this._transaction.CommitAsync().ConfigureAwait(false);
        Dispose();
    }

    public async Task RollbackAsync()
    {
        await this._transaction.RollbackAsync().ConfigureAwait(false);
        Dispose();
    }

    public async Task SaveAsync() =>
        //_changeTrackerManager?.FixupEntities(_context);
        await this.Context.SaveChangesAsync().ConfigureAwait(false);
}