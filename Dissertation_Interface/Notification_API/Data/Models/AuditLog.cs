using System.ComponentModel.DataAnnotations;

namespace Notification_API.Data.Models;

public class AuditLog
{
    [Key]
    public long Id { get; set; }
    public DateTime? EventTimeStamp { get; set; }

    public string EventType { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Outcome { get; set; } = string.Empty;

    public string EventDescription { get; set; } = string.Empty;

    public string TargetEntity { get; set; } = string.Empty;
}