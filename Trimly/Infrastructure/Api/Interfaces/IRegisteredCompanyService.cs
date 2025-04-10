using Microsoft.AspNetCore.Components.Forms;
using Trimly.Core.Application.DTOs;
using Trimly.Core.Application.DTOs.Companies;

namespace Trimly.Infrastructure.Api;

public interface IRegisteredCompanyService
{
    Task<ApiResponse<CreateRegisteredCompaniesDTO>> CreateCompany(CreateRegisteredCompaniesDTO company);
    Task<List<RegisteredCompaniesDTO>> GetRegisteredCompaniesById(List<string> companyIds);
}