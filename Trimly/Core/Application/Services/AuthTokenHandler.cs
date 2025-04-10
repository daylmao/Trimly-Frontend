using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

public class AuthTokenHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;
    private readonly NavigationManager _navigation;
    private readonly ILogger<AuthTokenHandler> _logger;

    public AuthTokenHandler(
        ILocalStorageService localStorage,
        NavigationManager navigation,
        ILogger<AuthTokenHandler> logger)
    {
        _localStorage = localStorage;
        _navigation = navigation;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        if (!request.RequestUri.PathAndQuery.Contains("/account/authenticate"))
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var jwtToken = new JwtSecurityToken(token);
                    if (jwtToken.ValidTo < DateTime.UtcNow)
                    {
                        _logger.LogWarning("Token expirado detectado");
                        await _localStorage.RemoveItemAsync("authToken");
                        _navigation.NavigateTo("/login", forceLoad: true);
                        return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    }

                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                catch
                {
                    await _localStorage.RemoveItemAsync("authToken");
                    _navigation.NavigateTo("/login", forceLoad: true);
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }
            }
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Respuesta no autorizada recibida");
            await _localStorage.RemoveItemAsync("authToken");
            _navigation.NavigateTo("/login", forceLoad: true);
        }

        return response;
    }
}