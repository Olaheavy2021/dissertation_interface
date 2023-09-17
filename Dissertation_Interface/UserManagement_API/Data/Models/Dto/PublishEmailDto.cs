namespace UserManagement_API.Data.Models.Dto;

public class PublishEmailDto
{
    public string CallbackUrl { get; set; } = string.Empty;

    public UserDto? User { get; set; }
}