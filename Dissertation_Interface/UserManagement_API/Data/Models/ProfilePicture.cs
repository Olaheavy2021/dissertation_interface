using UserManagement_API.Data.DomainHelper;

namespace UserManagement_API.Data.Models;

public class ProfilePicture : AuditableEntity<long>
{
    public string ImageData { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;
    // Foreign Key
    public string UserId { get; set; } = string.Empty;
    // Navigation Property
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