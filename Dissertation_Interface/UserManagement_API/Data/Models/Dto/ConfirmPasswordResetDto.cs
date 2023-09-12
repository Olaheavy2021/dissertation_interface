namespace UserManagement_API.Data.Models.Dto;

public class ConfirmPasswordResetDto
{
    public string Password { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Token { get; set; }
}