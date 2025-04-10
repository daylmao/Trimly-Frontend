using Trimly.Core.Application.Utils;

namespace Trimly.Core.Application.DTOs.Register;

public class RegisterRequestDTO
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}