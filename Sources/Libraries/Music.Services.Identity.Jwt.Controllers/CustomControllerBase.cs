using Microsoft.AspNetCore.Mvc;
using Music.Shared.Identity.Common;
using static System.Enum;

namespace Music.Services.Identity.Jwt.Controllers;

public abstract class CustomControllerBase : Controller
{
    protected Guid GetUserId()
    {
        return Guid.Parse(User.Claims.First(i => i.Type == "UserId").Value);
    }

    protected IReadOnlyCollection<Roles> GetRoles()
    {
        var roles = new List<Roles>();
        var rolesInToken = User.Claims.Where(x => x.Type == "Role");
        foreach (var claim in rolesInToken)
        {
            if(!TryParse<Roles>(claim.Value, out var role) || roles.Contains(role)) continue;
            roles.Add(role);            
        }
        return roles;
    }
}