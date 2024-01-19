using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Notification_API.Data;

namespace Notification_API.Extensions;
[ExcludeFromCodeCoverage]
public static class DbMigrator
{
    public static void ApplyMigration(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        NotificationDbContext db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

        if (db.Database.GetPendingMigrations().Any())
        {
            db.Database.Migrate();
        }
    }
}