using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Music.Shared.Identity.Common;

namespace Music.Services.Identity.Common;

public static class RolesHelper
{
    public static bool IsAdmin(this HttpContext context) =>
        context.User.HasClaim(claim => claim.Type == ClaimTypes.Role && claim.Value == Roles.Admin.ToString());
    
    public static bool IsArtist(this HttpContext context) =>
        context.User.HasClaim(claim => claim.Type == ClaimTypes.Role && claim.Value == Roles.Artist.ToString());
}