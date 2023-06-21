using Music.Shared.DTOs.Common;
using Music.Shared.Identity.Common.Models;

namespace Music.Shared.Identity.Jwt;

public record UpdateTokenRequest(
    string RefreshToken, 
    DeviceInfo DeviceInfo) : IUpdateRequest;