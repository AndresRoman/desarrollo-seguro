using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApi.Data.Models;
using WebApi.Services.Commond;

namespace WebApi.Services.Auth;

public class AuthService(IHttpContextAccessor httpContextAccessor, IOptions<JwtSettings> jwtSettings) : IAuthService
{
    private JwtSettings JwtSettings { get; } = jwtSettings.Value;

    public string GetSessionUser()
    {
        var username = httpContextAccessor.HttpContext!.User.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        return username!;
    }

    public string CreateToken(User usuario, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Name, usuario.FullName),
            new(JwtRegisteredClaimNames.NameId, usuario.Id!),
            new(JwtRegisteredClaimNames.UniqueName, usuario.UserName!),
            new(JwtRegisteredClaimNames.Email, usuario.Email!),
            new("userId", usuario.Id),
        };

        claims.AddRange(roles.Select(rol => new Claim(ClaimTypes.Role, rol)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings.Key!));

        var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescription = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(JwtSettings.ExpireTime),
            SigningCredentials = credenciales
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescription);

        return tokenHandler.WriteToken(token);
    }
}