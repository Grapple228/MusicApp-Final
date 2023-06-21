using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music.Identity.Service.Services;
using Music.Services.Identity.Common;
using Music.Shared.Identity.Common.DTOs;
using Music.Shared.Identity.Common.Models;
using Music.Shared.Identity.Common.Requests;
using Music.Shared.Identity.Jwt;

namespace Music.Identity.Service.Controllers;

[Authorize]
[ApiController]
[Route("api/[Controller]")]
public class IdentityController : ControllerBase
{
    private readonly IdentityService _identityService;
    private readonly ITokensService _tokensService;
    private readonly IUsersService _usersService;

    public IdentityController(IdentityService identityService, ITokensService tokensService, IUsersService usersService)
    {
        _identityService = identityService;
        _tokensService = tokensService;
        _usersService = usersService;
    }

    [AllowAnonymous]
    [HttpPost("Registration")]
    public async Task<ActionResult<RegisterDto>> RegisterUser(RegisterRequest request)
    {
        return await _identityService.Register(request);
    }
    
    [AllowAnonymous]
    [HttpPost("Authorization")]
    public async Task<ActionResult<LoginDto>> LoginUser(LoginRequest request)
    {
        return await _identityService.Login(request);
    }

    [AllowAnonymous]
    [HttpPost("RefreshToken")]
    public async Task<ActionResult<LoginDto>> RefreshToken(UpdateTokenRequest request)
    {
        return await _tokensService.RefreshToken(request);
    }
    
    [HttpGet("CheckAccess")]
    public ActionResult CheckAccess()
    {
        return NoContent();
    }
    
    [HttpDelete("{userId:guid}")]
    [Authorize(Policy = Policies.AdminPolicy)]
    public async Task<IActionResult> DeleteUserById(Guid userId)
    {
        await _usersService.DeleteUser(userId);
        return NoContent();
    }
    
    [HttpPut("{userId:guid}/ChangeUsername/{username}")]
    [Authorize(Policy = Policies.AdminPolicy)]
    public async Task<IActionResult> ChangeUserUsername(Guid userId, string username)
    {
        await _usersService.ChangeUsername(username, userId);
        return NoContent();
    }
    
    [HttpPost("{userId:guid}/ChangePassword")]
    [Authorize(Policy = Policies.AdminPolicy)]
    public async Task<IActionResult> ChangeUserPassword(Guid userId, ChangePasswordRequest request)
    {
        await _identityService.ChangePassword(request, userId, HttpContext);
        return NoContent();
    }

    [HttpGet("{userId:guid}")]
    [Authorize(Policy = Policies.AdminPolicy)]
    public async Task<IdentityUserDto> GetUser(Guid userId)
    {
        return await _usersService.GetUser(userId);
    }
    
    [HttpGet("")]
    [Authorize(Policy = Policies.AdminPolicy)]
    public async Task<IEnumerable<IdentityUserDto>> GetUsers()
    {
        return await _usersService.GetUsers();
    }
}