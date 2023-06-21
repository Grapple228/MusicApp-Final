using Music.Shared.Common;
using Music.Shared.DTOs.Common;
using Music.Shared.Identity.Common;

namespace Music.Shared.Identity.Jwt;

public record LoginDto(
    Guid Id, 
    string Username, 
    string ImagePath, 
    string JwtAccessToken, 
    string JwtRefreshToken,
    IReadOnlyCollection<Roles> Roles) 
    : ImageRecordBase(ImagePath), IModel;
