using Music.Shared.Identity.Common;

namespace Music.Shared.Identity.Jwt;

// TODO ADD USERNAME
public record TokenDto(
    string JwtAccessToken, 
    string JwtRefreshToken,
    IReadOnlyCollection<Roles> Roles);