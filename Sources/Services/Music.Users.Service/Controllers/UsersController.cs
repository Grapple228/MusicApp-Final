using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music.Shared.Common;
using Music.Shared.DTOs.Users;
using Music.Users.Service.Services;

namespace Music.Users.Service.Controllers;

[Authorize]
[Route("api/[Controller]")]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;

    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    
    //GET /api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto?>>> GetAsync()
    {
        return Ok(await _usersService.GetAll());
    }

    //GET /api/users/{pageNumber}-{countPerPage}
    [HttpGet("{pageNumber:long}-{countPerPage:long}")]
    public async Task<ActionResult<PageResult<UserDto>>> GetAsync([Range(1, long.MaxValue)] long pageNumber, [Range(1, long.MaxValue)] long countPerPage)
    {
        return await _usersService.GetPage(pageNumber, countPerPage);
    }
    
    //GET /api/users/{userId}
    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<UserDto>> GetByIdAsync(Guid userId)
    {
        return await _usersService.Get(userId);
    }
}