using System.Net.Http.Headers;
using System.Net.Http.Json;
using Trimly.Core.Application.DTOs;
using Trimly.Core.Application.DTOs.Schedules;
using Trimly.Core.Application.DTOs.Services;

namespace Trimly.Infrastructure.Api.Services;

public class ServicesHttp: IServices
{
    private readonly HttpClient _httpClient;
    public ServicesHttp(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<ApiResponse<ServicesDTos>> CreateService(CreateServiceDto request)
    {
        
        using var content = new MultipartFormDataContent();

        content.Add(new StringContent(request.Name ?? string.Empty), "Name");
        content.Add(new StringContent(request.Price.ToString()), "Price");
        content.Add(new StringContent(request.Description), "Description");
        content.Add(new StringContent(request.DurationInMinutes.ToString()), "DurationInMinutes");
        content.Add(new StringContent(request.RegisteredCompanyId.ToString()), "RegisteredCompanyId");
        
        if (request.ImageFile != null)
        {
            const long maxFileSize = 5 * 1024 * 1024;
            var fileStream = request.ImageFile.OpenReadStream(maxFileSize);
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(request.ImageFile.ContentType);
            content.Add(fileContent, "ImageFile", request.ImageFile.Name);
        }
        
        var response = await _httpClient.PostAsync($"api/v1/services", content);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ServicesDTos>();
            return new ApiResponse<ServicesDTos>()
            {
                Success = true,
                Data = result
            };
        }

        var validationMessages = await response.Content.ReadFromJsonAsync<List<ValidationMessage>>();
        return new ApiResponse<ServicesDTos>()
        {
            Success = false,
            ValidationMessages = validationMessages
        };

    }

    public async Task<ApiResponse<List<ServicesDTos>>> GetServicesByCompanyId(Guid companyId)
    {
        var response = await _httpClient.GetAsync($"api/v1/services/companies/{companyId}/services");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<List<ServicesDTos>>();
            return new ApiResponse<List<ServicesDTos>>()
            {
                Success = true,
                Data = result
            };
        }
        var validationMessages = await response.Content.ReadFromJsonAsync<List<ValidationMessage>>();
        return new ApiResponse<List<ServicesDTos>>()
        {
            Success = false,
            ValidationMessages = validationMessages
        };
    }
}