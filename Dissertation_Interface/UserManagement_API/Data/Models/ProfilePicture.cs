using System.Text.Json.Serialization;
using UserManagement_API.Data.DomainHelper;

namespace UserManagement_API.Data.Models;

public class ProfilePicture : AuditableEntity<long>
{
    public string ImageData { get; set; }

    public string Name { get; set; }

    public string ContentType { get; set; }
    // Foreign Key
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