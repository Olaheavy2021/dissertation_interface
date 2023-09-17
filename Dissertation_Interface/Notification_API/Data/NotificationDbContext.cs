using Microsoft.EntityFrameworkCore;
using Notification_API.Data.Models;

namespace Notification_API.Data;

public class NotificationDbContext:  DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
    {
    }

    public DbSet<EmailLogger> EmailLoggers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<EmailLogger>()
            .HasIndex(e => e.EmailIdentifier)
            .IsUnique();
    }
}