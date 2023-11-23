using Dissertation.Domain.DomainHelper;
using Dissertation.Domain.Entities;
using Dissertation.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Dissertation.Infrastructure.Context;

public class DissertationDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public DissertationDbContext(DbContextOptions<DissertationDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options) => this._httpContextAccessor = httpContextAccessor;

    public DbSet<AcademicYear> AcademicYears { get; set; }

    public DbSet<DissertationCohort> DissertationCohorts { get; set; }

    public DbSet<Department> Departments { get; set; }

    public DbSet<Course> Courses { get; set; }

    public DbSet<Supervisor> Supervisors { get; set; }

    public DbSet<SupervisorInvite> SupervisorInvites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) => ModelBuilderConfiguration.Configure(modelBuilder);

    public override int SaveChanges()
    {
        OnBeforeSaveChanges();
        var result = base.SaveChanges();
        return result;
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        OnBeforeSaveChanges();
        Task<int> result = base.SaveChangesAsync(cancellationToken);
        return result;
    }

    private void OnBeforeSaveChanges()
    {
        DateTime now = DateTime.UtcNow;
        var currentUserEmail = this._httpContextAccessor.HttpContext?.Items["Email"] as string;

        foreach (EntityEntry entry in ChangeTracker.Entries().Where(e => e.Entity.GetType().BaseType == typeof(AuditableEntity<long>)))
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    ((AuditableEntity<long>)entry.Entity).CreatedAt = now;
                    ((AuditableEntity<long>)entry.Entity).CreatedBy = currentUserEmail;
                    break;

                case EntityState.Modified:
                    ((AuditableEntity<long>)entry.Entity).UpdatedAt = now;
                    ((AuditableEntity<long>)entry.Entity).UpdatedBy = currentUserEmail;
                    break;
                case EntityState.Detached:
                    break;
                case EntityState.Unchanged:
                    break;
                case EntityState.Deleted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}