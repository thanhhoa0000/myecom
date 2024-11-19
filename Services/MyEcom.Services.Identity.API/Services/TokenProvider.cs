using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MyEcom.Services.Identity.API.Services.IServices;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace MyEcom.Services.Identity.API.Services;

internal sealed class TokenProvider : ITokenProvider
{
    public string CreateToken(AppUser user, IEnumerable<AppRoles> roles)
    {
        string secretKey = Environment.GetEnvironmentVariable("JWT_SECRET")!;
        var key = Encoding.ASCII.GetBytes(secretKey);
        var securityKey = new SymmetricSecurityKey(key);
        
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        };
        claims.AddRange(roles.Select(role 
            => new Claim(ClaimTypes.Role, role.ToString())));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime
                .UtcNow
                .AddMinutes(int
                    .TryParse(
                        Environment.GetEnvironmentVariable("JWT_EXPIRATION_IN_MINUTE"),
                        out var expiration)
                    ? expiration
                    : 0),

            SigningCredentials = credentials,
            Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
            Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
        };
        
        var handler = new JsonWebTokenHandler();
        string token = handler.CreateToken(tokenDescriptor);

        return token;
    }
}