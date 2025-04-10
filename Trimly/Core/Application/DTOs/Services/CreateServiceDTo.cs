using Microsoft.AspNetCore.Components.Forms;

namespace Trimly.Core.Application.DTOs.Services;

public class CreateServiceDto
{
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int DurationInMinutes { get; set; }
    public IBrowserFile? ImageFile { get; set; } 
    public Guid? RegisteredCompanyId { get; set; }
}