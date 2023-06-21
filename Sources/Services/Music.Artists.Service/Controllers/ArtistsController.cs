using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music.Artists.Service.Services;
using Music.Shared.Common;
using Music.Shared.DTOs.Artists;

namespace Music.Artists.Service.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ArtistsController : ControllerBase
{
    private readonly IArtistsService _artistsService;

    public ArtistsController(IArtistsService artistsService)
    {
        _artistsService = artistsService;
    }

    //GET /api/artists/pageNumber-countPerPage
    [HttpGet("{pageNumber:long}-{countPerPage:long}")]
    public async Task<ActionResult<PageResult<ArtistDto>>> GetPageAsync([Range(1, long.MaxValue)] long pageNumber, [Range(1, 1000)] long countPerPage = 30,
        string? searchQuery = null)
    {
        return Ok(await _artistsService.GetPage(pageNumber, countPerPage, searchQuery));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ArtistDto>>> GetAll([FromQuery]string? searchQuery, [FromQuery][Range(1, 1000)]long count = 30)
    {
        var media = await _artistsService.GetAll(searchQuery: searchQuery, count: count);
        return Ok(media);
    }
    
    

    //GET /api/artists/{artistId}
    [HttpGet("{artistId:guid}")]
    public async Task<ActionResult<ArtistDto>> GetByIdAsync(Guid artistId)
    {
        return await _artistsService.Get(artistId);
    }
}