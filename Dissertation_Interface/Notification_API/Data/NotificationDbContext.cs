using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Notification_API.Data.Models;

namespace Notification_API.Data;

[ExcludeFromCodeCoverage]
public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
    {
    }

    public DbSet<EmailLogger> EmailLoggers { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<EmailLogger>()
            .HasIndex(e => e.EmailIdentifier)
            .IsUnique();
    }
}