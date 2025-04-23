using System.Net.Http.Json;
using System.Text.Json;
using Blazored.LocalStorage;
using Trimly.Core.Application.DTOs;
using Trimly.Core.Application.DTOs.Appointment;
using Trimly.Core.Application.DTOs.Reviews;
using Trimly.Core.Application.DTOs.Schedules;
using Trimly.Core.Application.Pagination;

namespace Trimly.Infrastructure.Api.Services;

public class ReviewsService : IReviewsService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public ReviewsService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }
    
public async Task<ApiResponse<ReviewsDTos>> CreateAsync(CreateReviewsDTO request)
{
    try
    {
        var response = await _httpClient.PostAsJsonAsync("api/v1/review", request);
        var responseContent = await response.Content.ReadAsStringAsync(); // Leer como string primero

        if (response.IsSuccessStatusCode)
        {
            try
            {
                var content = JsonSerializer.Deserialize<ReviewsDTos>(responseContent);
                return new ApiResponse<ReviewsDTos>()
                {
                    Success = true,
                    Data = content
                };
            }
            catch (JsonException)
            {
                return new ApiResponse<ReviewsDTos>()
                {
                    Success = false,
                    Error = "La respuesta del servidor tiene un formato inválido"
                };
            }
        }

        // Manejo de errores mejorado
        try
        {
            // Primero intentar como lista de ValidationMessage
            var validationMessages = JsonSerializer.Deserialize<List<ValidationMessage>>(responseContent);
            if (validationMessages != null && validationMessages.Any())
            {
                return new ApiResponse<ReviewsDTos>()
                {
                    Success = false,
                    ValidationMessages = validationMessages,
                    Error = "Validation failed"
                };
            }

            // Si no es lista de validación, intentar como mensaje de error simple
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent);
            return new ApiResponse<ReviewsDTos>()
            {
                Success = false,
                Error = errorResponse?.Message ?? $"Error: {response.StatusCode}"
            };
        }
        catch (JsonException)
        {
            // Si no se puede parsear, devolver el contenido crudo
            return new ApiResponse<ReviewsDTos>()
            {
                Success = false,
                Error = responseContent ?? $"Error: {response.StatusCode}"
            };
        }
    }
    catch (Exception ex)
    {
        return new ApiResponse<ReviewsDTos>()
        {
            Success = false,
            Error = $"Exception: {ex.Message}"
        };
    }
}

public async Task<ApiResponse<PagedResponse<ReviewsDTos>>> PaginationReviews(int pageNumber, int pageSize)
{
    try
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;
        
        var requestUri = $"api/v1/review/pagination?pageNumber={pageNumber}&pageSize={pageSize}";
        
        var response = await _httpClient.GetAsync(requestUri);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            try
            {
                var pagedData = JsonSerializer.Deserialize<PagedResponse<ReviewsDTos>>(responseContent);
                return new ApiResponse<PagedResponse<ReviewsDTos>>
                {
                    Success = true,
                    Data = pagedData ?? new PagedResponse<ReviewsDTos>()
                };
            }
            catch (JsonException ex)
            {
                return new ApiResponse<PagedResponse<ReviewsDTos>>
                {
                    Success = false,
                    Error = $"Error deserializando respuesta: {ex.Message}"
                };
            }
        }

        // Manejo de error mejorado
        try
        {
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent);
            return new ApiResponse<PagedResponse<ReviewsDTos>>
            {
                Success = false,
                Error = errorResponse?.Message ?? $"Error: {response.StatusCode}"
            };
        }
        catch
        {
            return new ApiResponse<PagedResponse<ReviewsDTos>>
            {
                Success = false,
                Error = responseContent ?? $"Error: {response.StatusCode}"
            };
        }
    }
    catch (Exception ex)
    {
        return new ApiResponse<PagedResponse<ReviewsDTos>>
        {
            Success = false,
            Error = $"Error inesperado: {ex.Message}"
        };
    }
}

    public class ErrorResponse
    {
        public string Message { get; set; }
        public string Type { get; set; }
    }
}