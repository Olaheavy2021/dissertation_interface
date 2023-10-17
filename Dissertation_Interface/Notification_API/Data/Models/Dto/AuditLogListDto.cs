namespace Notification_API.Data.Models.Dto;

public class AuditLogListDto
{
    public long Id { get; set; }

    public DateTime? EventTimeStamp { get; set; }

    public string EventType { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Outcome { get; set; } = string.Empty;
}