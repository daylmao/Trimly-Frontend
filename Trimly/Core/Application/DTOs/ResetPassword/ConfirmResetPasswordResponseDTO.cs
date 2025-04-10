namespace Trimly.Core.Application.DTOs.ResetPassword;

public class ConfirmResetPasswordResponseDTO
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? Error { get; set; }

}