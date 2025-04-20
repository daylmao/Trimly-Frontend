using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Trimly.Core.Application.DTOs;
using Trimly.Core.Application.DTOs.Account;
using Trimly.Core.Application.DTOs.Auth;
using Trimly.Core.Application.DTOs.Register;
using Trimly.Core.Application.DTOs.ResetPassword;
using Trimly.Core.Application.DTOs.Schedules;
using Trimly.Core.Application.Interfaces.Services;

namespace Trimly.Core.Application.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly ILogger<AuthService> _logger;
    private readonly CustomAuthStateProvider _customAuthStateProvider;

    public AuthService(
        HttpClient httpClient, 
        ILocalStorageService localStorage,
        ILogger<AuthService> logger,
        CustomAuthStateProvider customAuthStateProvider)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _logger = logger;
        _customAuthStateProvider = customAuthStateProvider;
    }
    
    public async Task<string?> AuthenticateAsync(string email, string password)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/v1/account/authenticate", 
                new { email, password });

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error en autenticación: {StatusCode} - {Error}", 
                    response.StatusCode, errorContent);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<AuthenticateResponseDTO>();
        
            if (string.IsNullOrWhiteSpace(result?.Data?.JwtToken))
            {
                _logger.LogError("Token JWT vacío o inválido en la respuesta");
                return null;
            }

            if (!IsValidJwtStructure(result.Data.JwtToken))
            {
                _logger.LogError("Token JWT con estructura inválida");
                return null;
            }

            await _localStorage.SetItemAsync("authToken", result.Data.JwtToken);
            await _customAuthStateProvider.MarkUserAsAuthenticated(result.Data.JwtToken);
        
            return result.Data.JwtToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante autenticación");
            return null;
        }
    }
    
    public async Task LogoutAsync()
    {
        await _customAuthStateProvider.ClearAuthDataAsync();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var authToken = await _localStorage.GetItemAsync<string>("authToken");
        if (string.IsNullOrEmpty(authToken)) return false;

        var jwtHandler = new JwtSecurityTokenHandler();
        var token = jwtHandler.ReadJwtToken(authToken);
        return token.ValidTo > DateTime.UtcNow;
    }
    
    public async Task<ApiResponse<RegisterResponseDTO>> RegisterAsync(string endpoint, RegisterRequestDTO request)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/v1/account/{endpoint}", request);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<RegisterResponseDTO>();

            return new ApiResponse<RegisterResponseDTO>()
            {
                Success = true,
                Data = result
            };

        }
        
        var validationResponse = await response.Content.ReadFromJsonAsync<ValidationResponse>();
        return new ApiResponse<RegisterResponseDTO>
        {
            Success = false,
            ValidationMessages = validationResponse?.Errors,
            Error = "Validation failed"
        };
    }

    public async Task<ResetPasswordResponseDTO> ResetPasswordAsync(ResetPasswordRequestDTO request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/v1/account/forgot-password", request);

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadFromJsonAsync<ResetPasswordResponseDTO>();

            if (jsonResponse != null)
            {
                jsonResponse.Success = true;
                return jsonResponse;
            }

            var message = await response.Content.ReadAsStringAsync();
            return new ResetPasswordResponseDTO
            { 
                Success = true,
                Message = message 
            };
            
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        return new ResetPasswordResponseDTO
        {
            Success = false,
            Error = $"Error: {response.StatusCode}. {errorContent}"
        };
    }

    public async Task<ConfirmResetPasswordResponseDTO> ConfirmResetPasswordAsync(ConfirmResetPasswordRequestDTO request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/v1/account/reset-password", request);

        if (response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return new ConfirmResetPasswordResponseDTO
                {
                    Success = true,
                    Message = "Password reset successfully."
                };
            }

            var jsonResponse = await response.Content.ReadFromJsonAsync<ConfirmResetPasswordResponseDTO>();

            if (jsonResponse != null)
            {
                jsonResponse.Success = true;
                return jsonResponse;
            }

            var message = await response.Content.ReadAsStringAsync();
            return new ConfirmResetPasswordResponseDTO
            { 
                Success = true,
                Message = message 
            };
        }
    
        var errorContent = await response.Content.ReadAsStringAsync();
        return new ConfirmResetPasswordResponseDTO
        {
            Success = false,
            Error = $"Error: {response.StatusCode}. {errorContent}"
        };
    }
        
    public async Task<ConfirmAccountResponseDTO> ConfirmAccountAsync(string userId, string token)
    {
        try
        {
            var url = $"api/v1/account/confirm-account?userId={userId}&token={WebUtility.UrlEncode(token)}";
            var response = await _httpClient.PostAsync(url, null);

            if (response.IsSuccessStatusCode)
                return new ConfirmAccountResponseDTO { Success = true };

            var error = await response.Content.ReadAsStringAsync();
            return new ConfirmAccountResponseDTO
            {
                Success = false,
                Error = error
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tratando de confirmar la cuenta");
            return new ConfirmAccountResponseDTO
            {
                Success = false,
                Error = "La confirmacion ha fallado. Por favor intentalo de nuevo mas tarde"
            };
        }
        
    }

    public async Task<UpdateAccountResponseDTO> UpdateAccountProperties(UpdateAccountDTO request, string id)
    {
        var response = await _httpClient.PatchAsJsonAsync($"api/v1/account/{id}", request);
        if (response.IsSuccessStatusCode)
        {
            return new UpdateAccountResponseDTO
            {
                Success = true,
                Message = "Informacion actualizada exitosamente"
            };
        }
        var errorContent = await response.Content.ReadAsStringAsync();
        return new UpdateAccountResponseDTO()
        {
            Success = false,
            Error = errorContent,
        };

    }

    public async Task<ApiResponse<ProfileDTO>> GetAccountDetailsAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return new ApiResponse<ProfileDTO> 
            { 
                Success = false, 
                ErrorMessage = "UserId no puede estar vacío" 
            };
        }

        var response = await _httpClient.GetAsync($"api/v1/account/{userId}/details");
    
        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponse<ProfileDTO> 
            { 
                Success = false, 
                ErrorMessage = $"Error: {response.StatusCode}"
            };
        }

        var data = await response.Content.ReadFromJsonAsync<ProfileDTO>();
        return new ApiResponse<ProfileDTO> 
        { 
            Success = true, 
            Data = data 
        };
    }

    public async Task<ApiResponse<bool>> DeleteAccountById(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/v1/account/{id}");
        if (response.IsSuccessStatusCode)
        {
            return new ApiResponse<bool>()
            {
                Success = true,
                Data = true
            };
        }
        var errorContent = await response.Content.ReadAsStringAsync();
        return new ApiResponse<bool>()
        {
            Success = false,
            Error = errorContent
        };
    }

    private bool IsValidJwtStructure(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            handler.ReadJwtToken(token);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
}
