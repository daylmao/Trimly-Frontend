using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Trimly.Core.Application.DTOs.Auth;
using Trimly.Core.Application.DTOs.Register;
using Trimly.Core.Application.DTOs.ResetPassword;
using Trimly.Core.Application.Interfaces.Services;

namespace Trimly.Core.Application.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly ILogger<AuthService> _logger;
    private CustomAuthStateProvider _customAuthStateProvider;

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
      
        var response = await _httpClient.PostAsJsonAsync("api/v1/account/authenticate", new { email, password });
            
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Autenticación fallida. Código: {StatusCode}", response.StatusCode);
            return null;
        }

        var result = await response.Content.ReadFromJsonAsync<AuthenticateResponseDTO>();
            
        if (result?.Data?.JwtToken is not { Length: > 0 } token)
        {
            _logger.LogWarning("Token JWT vacío o inválido");
            return null;
        }

        await _customAuthStateProvider.MarkUserAsAuthenticated(token);
        return token;

    }
    
    public async Task LogoutAsync()
    {
        await _customAuthStateProvider.ClearAuthDataAsync();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var authToken = await _localStorage.GetItemAsync<string>("authToken");
        if (string.IsNullOrEmpty(authToken))
        {
            return false;
        }

        var jwtHandler = new JwtSecurityTokenHandler();
        var token = jwtHandler.ReadJwtToken(authToken);
        var expiration = token.ValidTo;

        return expiration > DateTime.UtcNow;
    }
    
    public async Task<RegisterResponseDTO> RegisterAsync(string endpoint, RegisterRequestDTO request)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/v1/account/{endpoint}", request);

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<RegisterResponseDTO>()
                   ?? new RegisterResponseDTO { Success = false, Error = "Formato de respuesta invalido" };

        var errorContent = await response.Content.ReadAsStringAsync();
        return new RegisterResponseDTO 
        { 
            Success = false, 
            Error = $"Error: {response.StatusCode}. {errorContent}"
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
}
