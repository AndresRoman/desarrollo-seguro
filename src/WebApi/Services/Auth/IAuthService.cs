using WebApi.Data.Models;

namespace WebApi.Services.Auth;

public interface IAuthService
{
    string GetSessionUser();

    string CreateToken(User usuario, IEnumerable<string> roles);
}