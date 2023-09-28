using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Notification_API.Data.Models;

public class EmailLogger
{
    [Key]
    public long Id { get; set; }
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    [StringLength(50)]
    public string Status { get; set; } = string.Empty;
    [StringLength(50)]
    public string EmailType { get; set; } = string.Empty;
    public DateTime? EmailSent { get; set; }

    public DateTime? EmailLogged { get; set; }
    public string EmailIdentifier { get; set; } = string.Empty;
}