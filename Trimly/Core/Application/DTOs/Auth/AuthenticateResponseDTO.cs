namespace Trimly.Core.Application.DTOs.Auth;

public class AuthenticateResponseDTO
{
    public bool Success { get; set; }
    public AuthenticateDataDTO? Data { get; set; }
    public string? ErrorMessage { get; set; }
}