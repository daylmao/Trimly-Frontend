namespace Trimly.Core.Application.DTOs.ResetPassword;

public class ConfirmResetPasswordRequestDTO
{
    public string? Email { get; set; }
    public string? Token { get; set; }
    public string? ConfirmPassword { get; set; }
    public string? Password { get; set; }
}