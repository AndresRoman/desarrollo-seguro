using Microsoft.AspNetCore.Identity;

namespace WebApi.Dtos;

public class UserResponse
{
    public string FullName { get; set; } = null!;
    public string AccessToken { get; set; } = null!;
    public string Username { get; set; } = null!;
    public List<string> Roles { get; set; } = null!;
}