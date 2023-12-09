using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UserManagement_API.Data.DomainHelper;
using UserManagement_API.Data.Models;

namespace UserManagement_API.Data;

public class UserDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UserDbContext(DbContextOptions<UserDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options) => this._httpContextAccessor = httpContextAccessor;

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    public DbSet<SupervisionCohort> SupervisionCohorts { get; set; }

    public DbSet<SupervisionRequest> SupervisionRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserDbContext).Assembly);

        // Configure the relationship between SupervisionCohort and ApplicationUser
        modelBuilder.Entity<SupervisionCohort>()
            .HasOne(sc => sc.Supervisor)
            .WithMany(u => u.SupervisedCohorts)
            .HasForeignKey(sc => sc.SupervisorId)
            .IsRequired();
    }

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
        var currentUserEmail = this._httpContextAccessor.HttpContext?.Items["Email"] as string ?? "system";

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