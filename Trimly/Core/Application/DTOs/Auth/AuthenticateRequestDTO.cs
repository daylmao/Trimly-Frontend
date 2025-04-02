namespace Trimly.Core.Application.DTOs.Auth;

public class AuthenticateRequestDTO
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}