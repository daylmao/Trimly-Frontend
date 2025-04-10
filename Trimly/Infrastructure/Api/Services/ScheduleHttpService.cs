using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Trimly.Core.Application.DTOs;
using Trimly.Core.Application.DTOs.Schedules;

namespace Trimly.Infrastructure.Api.Services;

public class ScheduleHttpService: IScheduleService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ScheduleHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }


    public async Task<ApiResponse<ScheduleDTO>> CreateSchedule(CreateScheduleDTO request)
    {
        
        var response = await _httpClient.PostAsJsonAsync($"api/v1/schedules", request);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ScheduleDTO>();
            return new ApiResponse<ScheduleDTO>()
            {
                Success = true,
                Data = result
            };
        }
        
        var validationMessages = await response.Content.ReadFromJsonAsync<List<ValidationMessage>>();
        return new ApiResponse<ScheduleDTO>()
        {
            Success = false,
            ValidationMessages = validationMessages,
            Error = "Validation failed"
        };
    }

    public async Task<ApiResponse<List<ScheduleDTO>>> GetSchedulesByCompany(Guid companyId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/v1/schedules/company/{companyId}/schedules");
            
            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<List<ScheduleDTO>> 
                { 
                    Success = false,
                    Error = $"Error: {response.StatusCode}"
                };
            }

            var content = await response.Content.ReadAsStringAsync();
            
            Console.WriteLine($"JSON recibido: {content}");
            
            var data = JsonSerializer.Deserialize<List<ScheduleDTO>>(content, _jsonOptions);
            
            return new ApiResponse<List<ScheduleDTO>> 
            { 
                Success = true, 
                Data = data 
            };
        }
        catch (JsonException ex)
        {
            return new ApiResponse<List<ScheduleDTO>> 
            { 
                Success = false,
                Error = $"Error al deserializar: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<ScheduleDTO>> 
            { 
                Success = false,
                Error = ex.Message
            };
        }
    }

    public async Task<DeleteResult> DeleteScheduleById(Guid scheduleId)
    {
        var response = await _httpClient.DeleteAsync($"api/v1/schedules/{scheduleId}");

        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            try
            {
                var result = JsonSerializer.Deserialize<DeleteResult>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return new DeleteResult
                {
                    DeletedId = result?.DeletedId
                };
            }
            catch (JsonException)
            {
                return new DeleteResult
                {
                    DeletedId = null,
                    ErrorMessage = "La respuesta del servidor no fue un JSON válido: " + content
                };
            }
        }

        return new DeleteResult
        {
            DeletedId = null,
            ErrorMessage = content
        };
    }

    public async Task<ApiResponse<ScheduleDTO>> UpdateScheduleById(Guid scheduleId, UpdateSchedulesDTos request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/v1/schedules/{scheduleId}", request);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ScheduleDTO>();
            return new ApiResponse<ScheduleDTO>
            {
                Success = true,
                Data = result
            };
        }

        var validationMessages = await response.Content.ReadFromJsonAsync<List<ValidationMessage>>();

        return new ApiResponse<ScheduleDTO>
        {
            Success = false,
            ValidationMessages = validationMessages,
            Error = "Validation failed"
        };
    }
}