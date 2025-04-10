namespace Trimly.Core.Application.DTOs.Barbers;

public class BarberDTO
{
    public string? UserId { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = false;

}