using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace blazor.Auth;

public class ApiAuthenticationStateProvider(
    HttpClient httpClient,
    ILocalStorageService localStorage)
    : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var savedToken = await localStorage.GetItemAsync<string>("authToken");

        if(string.IsNullOrEmpty(savedToken))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",savedToken);

        return new AuthenticationState(
                new ClaimsPrincipal(
                    new ClaimsIdentity(ParseClaimFromJwt(savedToken), "jwt")
                )
            );
    }

    public void MarkUserAsAuthenticated(string email)
    {
        var authenticatedUser = new ClaimsPrincipal(
            new ClaimsIdentity(new[] {new Claim(ClaimTypes.Name, email)}, "apiauth")
            );

        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
    }
    
    public void MarkUserAsLoggedOut()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));
        NotifyAuthenticationStateChanged(authState);
    }


    
    private static IEnumerable<Claim> ParseClaimFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split(".")[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        keyValuePairs!.TryGetValue(ClaimTypes.Role, out var roles);

        if(roles is not null)
        {
            if(roles.ToString()!.Trim().StartsWith("["))
            {
                var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString()!);
                claims.AddRange(parsedRoles!.Select(parsedRole => new Claim(ClaimTypes.Role, parsedRole)));
            }else{
                claims.Add(new Claim(ClaimTypes.Role, roles.ToString()!));
            }

            keyValuePairs.Remove(ClaimTypes.Role);
        }


        claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!)));


        return claims;
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch(base64.Length % 4)
        {
            case 2: base64 += "==";break;
            case 3: base64 += "="; break;
        }

        return Convert.FromBase64String(base64);
    }



}