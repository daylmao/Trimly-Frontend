using Microsoft.AspNetCore.Components.Forms;

namespace Trimly.Core.Application.DTOs.Companies;

public class CreateRegisteredCompaniesDTO
{
    public Guid? RegisteredCompaniesId { get; set; }
    public string? Name { get; set; }
    public string? Rnc { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AddresCompanies { get; set; }
    public string? Email { get; set; }
    public string? DescriptionCompanies { get; set; }
    public IBrowserFile? ImageFile { get; set; } 





}