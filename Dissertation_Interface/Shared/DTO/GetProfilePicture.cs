namespace UserManagement_API.Data.Models.Dto;

public class GetProfilePicture
{
    public string ImageData { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;
}