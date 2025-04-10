using Trimly.Core.Application.DTOs.Services;

namespace Trimly.Core.Application.DTOs.Companies;

public class Company
{
    public Guid RegisteredCompanyId { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Rnc { get; set; }
    public string AddresCompanies { get; set; }
    public string DescriptionCompanies { get; set; }
    public string ImageUrl { get; set; }
    public List<ServicesDTos> Services { get; set; }
}