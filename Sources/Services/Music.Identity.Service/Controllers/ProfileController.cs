using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music.Identity.Service.Services;
using Music.Shared.Identity.Common.Models;
using Music.Shared.Identity.Common.Requests;

namespace Music.Identity.Service.Controllers;

[Authorize]
[ApiController]
[Route("api/Identity/[Controller]")]
public class ProfileController : ControllerBase
{
    private readonly IUsersService _usersService;
    private readonly IIdentityService _identityService;

    public ProfileController(IUsersService usersService, IIdentityService identityService)
    {
        _usersService = usersService;
        _identityService = identityService;
    }

    [HttpGet]
    public async Task<ActionResult<IdentityUserDto>> GetProfile()
    {
        return await _usersService.GetUser(GetUserId());
    }
    
    [HttpGet("Id")]
    public ActionResult<Guid> GetId()
    {
        return GetUserId();
    }

    [HttpGet("Roles")]
    public ActionResult<IEnumerable<string>> GetUserRoles()
    {
        var roles = GetRoles().Select(x => x.ToString());
        return Ok(roles);
    }

    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        await _identityService.ChangePassword(request, GetUserId(), HttpContext);
        return NoContent();
    }

    [HttpPost("ChangeUsername/{username}")]
    public async Task<IActionResult> ChangeUsername(string username)
    {
        await _usersService.ChangeUsername(username, GetUserId());
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUser()
    {
        await _usersService.DeleteUser(GetUserId());
        return NoContent();
    }

    [HttpPost("BecomeArtist")]
    public async Task BecomeAnArtist()
    {
        // TODO ДОБАВИТЬ ЗАПРОС СТАТЬ ИСПОЛНИТЕЛЕМ
    }
}