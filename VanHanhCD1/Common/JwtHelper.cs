using VanHanhCD1.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace VanHanhCD1.Common
{
    public static class JwtHelper
    {
        public static string GenerateToken(IConfiguration config, NguoiDung user)
{
    var jwtConfig = config.GetSection("Jwt");

    string? keyStr = jwtConfig["Key"];
    string? issuer = jwtConfig["Issuer"];
    string? audience = jwtConfig["Audience"];
    string? expireMinutesStr = jwtConfig["ExpireMinutes"];

    if (string.IsNullOrEmpty(keyStr))
        throw new ArgumentException("JWT key is missing in configuration.");

    if (string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
        throw new ArgumentException("JWT issuer or audience is missing in configuration.");

    if (!int.TryParse(expireMinutesStr, out int expireMinutes))
        expireMinutes = 5; // fallback

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
        new Claim("UserId", user.IDNguoiDung.ToString()),
        new Claim("Username", user.TenDangNhap ?? ""),
        new Claim("Role", user.IDQuyen?.ToString() ?? "")
    };

    var token = new JwtSecurityToken(
        issuer: issuer,
        audience: audience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(expireMinutes),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

    }
}
