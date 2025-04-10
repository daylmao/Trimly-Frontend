namespace Trimly.Core.Application.DTOs.Account;

public class ProfileDTO
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime UpdateAt { get; set; }
}