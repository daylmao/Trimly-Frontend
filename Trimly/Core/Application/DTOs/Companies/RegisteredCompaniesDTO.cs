using Trimly.Core.Application.Utils;

namespace Trimly.Core.Application.DTOs.Companies;

public class RegisteredCompaniesDTO
{
    public Guid RegisteredCompaniesId { get; set; }
    public string? Name { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AddresCompanies { get; set; }
    public string? DescriptionCompanies { get; set; }
    public string? LogoUrl { get; set; }
    public string? Email { get; set; }
    public Status? Status { get; set; }
    public DateTime? RegistrationDateCompany { get; set; }
}