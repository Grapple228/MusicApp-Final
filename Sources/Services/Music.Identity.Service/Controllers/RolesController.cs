using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music.Identity.Service.Services;
using Music.Services.Identity.Common;
using Music.Shared.Identity.Common;

namespace Music.Identity.Service.Controllers;

[ApiController]
[Authorize(Policy = Policies.AdminPolicy)]
[Route("api/Identity/{userId:guid}/[Controller]")]
public class RolesController : ControllerBase
{
    private readonly IRolesService _rolesService;

    public RolesController(IRolesService rolesService)
    {
        _rolesService = rolesService;
    }

    [HttpGet("")]
    public async Task<IEnumerable<Roles>> GetUserRoles(Guid userId)
    {
        return await _rolesService.GetUserRoles(userId);
    }

    [HttpPost("{role}")]
    public async Task<IActionResult> AddRole(Guid userId, Roles role)
    {
        await _rolesService.AddRole(userId, role);
        return NoContent();
    }
    
    [HttpDelete("{role}")]
    public async Task<IActionResult> RemoveRole(Guid userId, Roles role)
    {
        await _rolesService.RemoveRole(userId, role);
        return NoContent();
    }
}