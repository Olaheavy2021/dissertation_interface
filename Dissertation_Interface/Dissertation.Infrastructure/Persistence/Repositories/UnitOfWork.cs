using Dissertation.Infrastructure.Context;
using Dissertation.Infrastructure.Persistence.IRepository;
using Microsoft.EntityFrameworkCore.Storage;

namespace Dissertation.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private IDbContextTransaction _transaction;
    public DissertationDbContext Context;

    public UnitOfWork(DissertationDbContext context) => this.Context = context;

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

    public IAcademicYearRepository AcademicYearRepository => new AcademicYearRepository(this.Context);

    public IDissertationCohortRepository DissertationCohortRepository => new DissertationCohortRepository(this.Context);

    public ICourseRepository CourseRepository => new CourseRepository(this.Context);

    public IDepartmentRepository DepartmentRepository => new DepartmentRepository(this.Context);

    public ISupervisorInviteRepository SupervisorInviteRepository => new SupervisorInviteRepository(this.Context);

    public ISupervisorRepository SupervisorRepository => new SupervisorRepository(this.Context);

    public IStudentRepository StudentRepository => new StudentRepository(this.Context);

    public IStudentInviteRepository StudentInviteRepository => new StudentInviteRepository(this.Context);

    public IResearchProposalRepository ResearchProposalRepository => new ResearchProposalRepository(this.Context);

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

    public async Task<int> SaveAsync() =>
        await this.Context.SaveChangesAsync(cancellationToken: default);

    public async Task<int> SaveAsync(CancellationToken cancellationToken) =>
        await this.Context.SaveChangesAsync(cancellationToken: cancellationToken);
}