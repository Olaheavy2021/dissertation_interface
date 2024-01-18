using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using UserManagement_API.Data.DomainHelper;

namespace UserManagement_API.Data.Models;

public class ProfilePicture : AuditableEntity<long>
{
    [MaxLength(100)]
    public string ImageData { get; set; }

    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(100)]
    public string ContentType { get; set; }

    // Foreign Key
    [MaxLength(100)]
    public string UserId { get; set; }

    // Navigation Property
    [JsonIgnore]
    public virtual ApplicationUser User { get; set; } = null!;

    private ProfilePicture(
        string imageData,
        string name,
        string contentType,
        string userId
    )
    {
        ImageData = imageData;
        Name = name;
        ContentType = contentType;
        UserId = userId;
    }

    public static ProfilePicture Create(
        string imageData,
        string name,
        string contentType,
        string userId) =>
        new(imageData, name, contentType, userId);
}