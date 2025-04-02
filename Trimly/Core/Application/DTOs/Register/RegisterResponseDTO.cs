namespace Trimly.Core.Application.DTOs.Register;

public class RegisterResponseDTO
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string UserId { get; set; }
}