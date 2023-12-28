using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using blazor.Model;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace blazor.Auth;

public class AuthService(
    HttpClient httpClient,
    AuthenticationStateProvider authenticationStateProvider,
    ILocalStorageService localStorage)
    : IAuthService
{
    public async Task<string?> Login(LoginModel loginModel)
    {
        
        var loginAsJson = JsonSerializer.Serialize(loginModel);
        var response = await  httpClient.PostAsync("api/auth/sign-in", 
                new StringContent(loginAsJson, Encoding.UTF8, "application/json"));

        if(!response.IsSuccessStatusCode)
        {
            return null!;
        }

        var loginResult = await response.Content.ReadAsStringAsync();
        var loginResultObject = JsonSerializer.Deserialize<UserResponse>(loginResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        var accessToken = loginResultObject!.AccessToken;
        await localStorage.SetItemAsStringAsync("authToken", accessToken);
        ((ApiAuthenticationStateProvider)authenticationStateProvider).MarkUserAsAuthenticated(loginModel.Email!);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",accessToken);

        return loginResult;
    }

    public async Task Logout()
    {
       await localStorage.RemoveItemAsync("authToken");
       ((ApiAuthenticationStateProvider)authenticationStateProvider).MarkUserAsLoggedOut();
       httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<string?> Register(RegisterModel registerModel)
    {
       var registerAsJson = JsonSerializer .Serialize(registerModel);

       var response =  await httpClient.PostAsync("api/auth/sing-up", new StringContent(registerAsJson,Encoding.UTF8, "application/json" )) ;

        if(!response.IsSuccessStatusCode)
        {
            return null!;
        }

        var registerResult = await response.Content.ReadAsStringAsync();
        return registerResult;
    }
}