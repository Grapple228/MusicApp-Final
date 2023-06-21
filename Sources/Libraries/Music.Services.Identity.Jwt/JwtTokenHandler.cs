using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Music.Shared.Identity.Common;
using Music.Shared.Identity.Jwt;

namespace Music.Services.Identity.Jwt;

public class JwtTokenHandler
{
    private static string? _key;
    public static SymmetricSecurityKey SecurityKey => new(
        Encoding.ASCII.GetBytes(_key ?? throw new Exception("Key is emplty! Use SetKey to initialize it.")));

    public JwtTokenHandler(double accessValidityMinutes, double refreshValidityDays)
    {
        AccessValidityMinutes = accessValidityMinutes;
        RefreshValidityDays = refreshValidityDays;
    }

    public static void SetKey(string key)
    {
        _key = key;
    }
    
    public double AccessValidityMinutes { get; }
    public double RefreshValidityDays { get; }
    
    public TokenDto CreateJwtToken(Guid userId, IReadOnlyCollection<Roles> userRoles)
    {
        var tokenExpiryTimeStamp = DateTime.Now.AddMinutes(AccessValidityMinutes);
        var claims = new List<Claim>()
        {
            new ("UserId", userId.ToString())
        };
        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role.ToString())));

        var claimsIdentity = new ClaimsIdentity(claims);

        var signingCredentials = new SigningCredentials(
            SecurityKey, SecurityAlgorithms.HmacSha256Signature);

        var securityTokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = claimsIdentity,
            Expires = tokenExpiryTimeStamp,
            SigningCredentials = signingCredentials,
        };

        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
        var token = jwtSecurityTokenHandler.WriteToken(securityToken);

        return new TokenDto(token, Convert.ToBase64String(
            GenRandomBytesArray(20)), userRoles.ToList());
    }

    public static byte[] GenRandomBytesArray(int length) =>
        RandomNumberGenerator.GetBytes(length);
}