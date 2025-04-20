using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Blazored.LocalStorage;
using Trimly.Core.Application.DTOs;
using Trimly.Core.Application.DTOs.Appointment;
using Trimly.Core.Application.DTOs.Schedules;
using Trimly.Core.Application.Pagination;
using Trimly.Core.Domain.Enums;

namespace Trimly.Infrastructure.Api.Services;

public class AppointmentHttpService : IAppointmentHttpService
{
    private readonly HttpClient _httpClient;
    private IAppointmentHttpService _appointmentHttpServiceImplementation;

    public AppointmentHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResponse<IEnumerable<AppointmentDTos>>> GetAppointmentsByStatusAsync(AppointmentStatus status)
    {
        var response = await _httpClient.GetAsync($"api/v1/appointments/search/status/{status}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            
            var appointments = JsonSerializer.Deserialize<IEnumerable<AppointmentDTos>>(content);
            
            return new ApiResponse<IEnumerable<AppointmentDTos>>()
            {
                Success = true,
                Data = appointments
            };
        }
        
        return new ApiResponse<IEnumerable<AppointmentDTos>>()
        {
            Success = false,
            Error = $"Error al obtener citas: {response.StatusCode}"
        };
    }
    
    public async Task<ApiResponse<string>> ConfirmAppointmentAsync(Guid appointmentId, Guid serviceId)
    {
        try
        {
            // Crear un objeto anónimo para enviar serviceId si es necesario
            var requestData = new { ServiceId = serviceId };
            var response = await _httpClient.PostAsJsonAsync($"api/v1/appointments/{appointmentId}/confirm", requestData);

            var rawContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Raw API response: {rawContent}");

            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(rawContent);
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Error = errorResponse?.Message ?? rawContent
                    };
                }
                catch
                {
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Error = rawContent
                    };
                }
            }
            
            return new ApiResponse<string>
            {
                Success = true,
                Data = rawContent.Trim('"') 
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex}");
            return new ApiResponse<string>
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    public async Task<ApiResponse<AppointmentResponse>> CreateAppointmentAsync(CreateAppointmentsDTos request)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/v1/appointments", request);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadFromJsonAsync<AppointmentResponse>();
            return new ApiResponse<AppointmentResponse>()
            {
                Success = true,
                Data = content
            };
        }

        var errorContent = await response.Content.ReadFromJsonAsync<List<ValidationMessage>>();
        return new ApiResponse<AppointmentResponse>()
        {
            Success = false,
            ValidationMessages = errorContent
        };
    }

    private class ErrorResponse
    {
        public string Message { get; set; }
    }

    public async Task<ApiResponse<bool>> CanceledAppointmentAsync(Guid appointmentId)
    {
        try
        {
            var response = await _httpClient.PostAsync(
                $"api/v1/appointments/cancel-appointment/{appointmentId}",
                new StringContent("", Encoding.UTF8, "application/json"));

            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse<bool>
                {
                    Success = true,
                    Data = true 
                };
            }
            
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content);
                return new ApiResponse<bool>
                {
                    Success = false,
                    Error = errorResponse?.Message ?? content
                };
            }
            catch
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Error = content
                };
            }
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Error = ex.Message
            };
        }
    }
    
  public async Task<ApiResponse<PagedResponse<AppointmentDTos>>> PaginationAppointmentsAsync(
    int pageNumber, 
    int pageSize)
{
    try
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var requestUri = $"api/v1/appointments/pagination?pageNumber={pageNumber}&pageSize={pageSize}";

        using var response = await _httpClient.GetAsync(requestUri);

        if (response.IsSuccessStatusCode)
        {
            var pagedData = await response.Content.ReadFromJsonAsync<PagedResponse<AppointmentDTos>>();
            
            return new ApiResponse<PagedResponse<AppointmentDTos>>
            {
                Success = true,
                Data = pagedData ?? new PagedResponse<AppointmentDTos>()
            };
        }

        return await HandleErrorResponse<PagedResponse<AppointmentDTos>>(response);
    }
    catch (Exception ex)
    {
        return new ApiResponse<PagedResponse<AppointmentDTos>>
        {
            Success = false,
            Error = $"Error inesperado: {ex.Message}"
        };
    }
}

private async Task<ApiResponse<T>> HandleErrorResponse<T>(HttpResponseMessage response)
{
    var errorContent = await response.Content.ReadAsStringAsync();

    try
    {
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(errorContent);
        return new ApiResponse<T>
        {
            Success = false,
            Error = errorResponse?.Message ?? $"Error del servidor ({(int)response.StatusCode})"
        };
    }
    catch
    {
        return new ApiResponse<T>
        {
            Success = false,
            Error = !string.IsNullOrEmpty(errorContent) 
                ? errorContent 
                : $"Error del servidor ({(int)response.StatusCode})"
        };
    }
}

}