namespace Shared.DTO;

public class PublishEmailDto
{
    public string CallbackUrl { get; set; } = string.Empty;
    public UserDto? User { get; set; }
    public string EmailType { get; set; } = string.Empty;
}