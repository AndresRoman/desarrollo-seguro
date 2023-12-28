using Microsoft.AspNetCore.Identity;

namespace WebApi.Data.Models;

public class User : IdentityUser
{
    public string FullName { get; set; } = null!;
}