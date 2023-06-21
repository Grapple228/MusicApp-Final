using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Music.Shared.Identity.Common;

namespace Music.Services.Identity.Common;

public static class PolicyHelper
{
    public static void AddPolicies(this AuthorizationOptions options)
    {
        options.AddPolicy(Policies.AdminPolicy, p => p.RequireClaim(ClaimTypes.Role, nameof(Roles.Admin)));
        options.AddPolicy(Policies.ArtistPolicy, p => p.RequireClaim(ClaimTypes.Role, nameof(Roles.Artist)));
        options.AddPolicy(Policies.UserPolicy, p => p.RequireClaim(ClaimTypes.Role, nameof(Roles.User)));
        options.AddPolicy(Policies.ArtistOrAdminPolicy, p =>
        {
            p.RequireAssertion(context =>
                context.User.HasClaim(c => c.Value 
                    is nameof(Roles.Admin) 
                    or nameof(Roles.Artist))
                    );
        });
    }
}