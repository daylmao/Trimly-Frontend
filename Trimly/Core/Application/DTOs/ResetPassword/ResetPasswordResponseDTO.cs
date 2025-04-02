namespace Trimly.Core.Application.DTOs.ResetPassword;

public class ResetPasswordResponseDTO
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string? Message { get; set; }
}