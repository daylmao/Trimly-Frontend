using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;

using Trimly.Core.Application.DTOs;
using Trimly.Core.Application.DTOs.Companies;

namespace Trimly.Infrastructure.Api.Services;

public class RegisteredCompanyHttpService : IRegisteredCompanyService
{
    private readonly CustomAuthStateProvider _customAuthStateProvider;
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient;

    public RegisteredCompanyHttpService(CustomAuthStateProvider customAuthStateProvider,
        ILocalStorageService localStorage, HttpClient httpClient)
    {
        _customAuthStateProvider = customAuthStateProvider;
        _localStorage = localStorage;
        _httpClient = httpClient;
    }

   public async Task<ApiResponse<CreateRegisteredCompaniesDTO>> CreateCompany(CreateRegisteredCompaniesDTO company)
    {
    try
    {
        using var content = new MultipartFormDataContent();

        content.Add(new StringContent(company.Name ?? string.Empty), "Name");
        content.Add(new StringContent(company.Rnc ?? string.Empty), "Rnc");
        content.Add(new StringContent(company.PhoneNumber ?? string.Empty), "PhoneNumber");
        content.Add(new StringContent(company.AddresCompanies ?? string.Empty), "AddresCompanies");
        content.Add(new StringContent(company.Email ?? string.Empty), "Email");
        content.Add(new StringContent(company.DescriptionCompanies ?? string.Empty), "DescriptionCompanies");

        if (company.ImageFile != null)
        {
            const long maxFileSize = 5 * 1024 * 1024;
            var fileStream = company.ImageFile.OpenReadStream(maxFileSize);
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(company.ImageFile.ContentType);
            content.Add(fileContent, "ImageFile", company.ImageFile.Name);
        }

        var response = await _httpClient.PostAsync("api/v1/registeredCompanies", content);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<CreateRegisteredCompaniesDTO>();
            return new ApiResponse<CreateRegisteredCompaniesDTO>
            {
                Success = true,
                Data = result
            };
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        return new ApiResponse<CreateRegisteredCompaniesDTO>
        {
            Success = false,
            ErrorMessage = $"Error: {response.StatusCode} - {errorContent}",
        };
    }
    catch (Exception ex)
    {
        return new ApiResponse<CreateRegisteredCompaniesDTO>
        {
            Success = false,
            ErrorMessage = $"Error inesperado: {ex.Message}",
        };
    }
    }   

    public async Task<List<RegisteredCompaniesDTO>> GetRegisteredCompaniesById(List<string> companyIds)
    {
        var companies = new List<RegisteredCompaniesDTO>();
    
        foreach (var companyId in companyIds)
        {
            if (string.IsNullOrWhiteSpace(companyId)) continue;
        
            var response = await _httpClient.GetAsync($"api/v1/registeredCompanies/{companyId}");
        
            if (response.IsSuccessStatusCode)
            {
                var company = await response.Content.ReadFromJsonAsync<RegisteredCompaniesDTO>();
                if (company != null)
                {
                    companies.Add(company);
                }
            }
        }
    
        return companies;
    }
}