using System.IdentityModel.Tokens.Jwt;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private const string AuthTokenKey = "authToken";
    private readonly ILocalStorageService _localStorage;

    public CustomAuthStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>(AuthTokenKey);
    
        if (string.IsNullOrEmpty(token)) 
            return CreateAnonymousState();

        try
        {
            var jwtToken = new JwtSecurityToken(token);
        
            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                await ClearAuthDataAsync();
                return CreateAnonymousState();
            }

            var claims = ParseClaimsFromJwt(token);
            var identity = CreateClaimsIdentity(claims);
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch
        {
            await ClearAuthDataAsync();
            return CreateAnonymousState();
        }
    }

    
    public async Task MarkUserAsAuthenticated(string token)
    {
        try
        {
            var jwtToken = new JwtSecurityToken(token);
            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                await ClearAuthDataAsync();
                return;
            }

            await _localStorage.SetItemAsync(AuthTokenKey, token);
            var claims = ParseClaimsFromJwt(token);
            var identity = CreateClaimsIdentity(claims);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
        }
        catch
        {
            await ClearAuthDataAsync();
        }
    }

    public async Task ClearAuthDataAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");
        await _localStorage.RemoveItemAsync("userId");
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    }

    public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        try
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = Base64UrlDecode(payload);
            using var jsonDoc = JsonDocument.Parse(jsonBytes);
            
            var claims = new List<Claim>();
            foreach (var prop in jsonDoc.RootElement.EnumerateObject())
            {
                if (prop.Name.Equals("roles", StringComparison.OrdinalIgnoreCase))
                {
                    AddRoleClaims(prop.Value, claims);
                    continue;
                }
                claims.Add(new Claim(prop.Name, prop.Value.ToString()));
            }

            EnsureNameClaimExists(claims);
            return claims;
        }
        catch
        {
            return Enumerable.Empty<Claim>();
        }
    }

    private static void AddRoleClaims(JsonElement roleElement, List<Claim> claims)
    {
        if (roleElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var role in roleElement.EnumerateArray())
            {
                claims.Add(new Claim(ClaimTypes.Role, role.GetString()!));
            }
        }
        else
        {
            claims.Add(new Claim(ClaimTypes.Role, roleElement.GetString()!));
        }
    }

    private static void EnsureNameClaimExists(List<Claim> claims)
    {
        if (!claims.Any(c => c.Type == ClaimTypes.Name))
        {
            var email = claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "unknown";
            claims.Add(new Claim(ClaimTypes.Name, email));
        }
    }

    private static ClaimsIdentity CreateClaimsIdentity(IEnumerable<Claim> claims)
    {
        return new ClaimsIdentity(
            claims,
            "jwt",
            ClaimTypes.Name,
            ClaimTypes.Role
        );
    }

    private static AuthenticationState CreateAnonymousState()
    {
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    private static byte[] Base64UrlDecode(string input)
    {
        return WebEncoders.Base64UrlDecode(input);
    }
    
    private bool IsTokenValid(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token)) return false;
        
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.ValidTo > DateTime.UtcNow;
        }
        catch
        {
            return false;
        }
    }
}